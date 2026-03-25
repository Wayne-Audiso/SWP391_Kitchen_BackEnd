using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Exceptions;
using BackendSWP391.Application.Models.StoreInventory;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Persistence;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class StoreInventoryService(
    IGenericRepository<StoreIngredientStock> stockRepo,
    IGenericRepository<StoreCostRecord>      costRepo,
    IGenericRepository<Shipment>             shipmentRepo,
    IGenericRepository<ShipmentLine>         shipmentLineRepo,
    IGenericRepository<Product>              productRepo,
    IGenericRepository<RecipeIngredient>     recipeIngredientRepo,
    IGenericRepository<Ingredient>           ingredientRepo,
    IGenericRepository<StoreOrder>           storeOrderRepo,
    DatabaseContext                          db) : IStoreInventoryService
{
    public async Task<List<StoreStockDto>> GetStoreStockAsync(int storeId)
    {
        return await stockRepo.Queryable
            .Include(s => s.Ingredient)
            .Where(s => s.StoreId == storeId)
            .Select(s => new StoreStockDto
            {
                Id             = s.Id,
                StoreId        = s.StoreId,
                IngredientId   = s.IngredientId,
                IngredientName = s.Ingredient.IngredientName,
                Unit           = s.Ingredient.Unit,
                CurrentStock   = s.CurrentStock,
                ExpiryDate     = s.ExpiryDate,
                MinStock       = s.Ingredient.MinStock,
                Price          = s.Ingredient.Price
            })
            .ToListAsync();
    }

    public async Task SellAtStoreAsync(int storeId, int productId, int quantity)
    {
        var product = await productRepo.FindAsync(productId)
            ?? throw new NotFoundException($"Không tìm thấy sản phẩm Id={productId}");

        if (product.RecipeId is null)
            throw new BadRequestException("Sản phẩm này chưa được liên kết với công thức nào");

        var recipeIngredients = await recipeIngredientRepo.Queryable
            .Where(ri => ri.RecipeId == product.RecipeId)
            .ToListAsync();

        if (!recipeIngredients.Any())
            throw new BadRequestException("Công thức không có nguyên liệu nào");

        // Kiểm tra tồn kho đủ không
        var insufficient = new List<string>();
        var deductions   = new List<(StoreIngredientStock stock, Ingredient ing, decimal required)>();

        foreach (var ri in recipeIngredients)
        {
            var required = (ri.Quantity ?? 0) * quantity;
            if (required == 0) continue;

            var ing = await ingredientRepo.FindAsync(ri.IngredientId);
            if (ing is null) continue;

            // Lấy stock đủ nhất (hạn gần nhất trước — FEFO)
            var stock = await stockRepo.Queryable
                .Where(s => s.StoreId == storeId && s.IngredientId == ri.IngredientId && s.CurrentStock > 0)
                .OrderBy(s => s.ExpiryDate == null ? DateTime.MaxValue : s.ExpiryDate.Value)
                .FirstOrDefaultAsync();

            var available = stock?.CurrentStock ?? 0;
            if (available < required)
                insufficient.Add($"{ing.IngredientName}: cần {required} {ing.Unit}, còn {available} {ing.Unit}");
            else
                deductions.Add((stock!, ing, required));
        }

        if (insufficient.Any())
            throw new BadRequestException($"Không đủ nguyên liệu: {string.Join("; ", insufficient)}");

        // Trừ kho và ghi OperatingCost
        foreach (var (stock, ing, required) in deductions)
        {
            stock.CurrentStock -= required;
            stock.UpdatedAt     = DateTime.UtcNow;
            await stockRepo.UpdateAsync(stock);

            await costRepo.AddAsync(new StoreCostRecord
            {
                StoreId      = storeId,
                IngredientId = ing.IngredientId,
                Quantity     = required,
                Cost         = required * (ing.Price ?? 0),
                CostType     = "OperatingCost",
                OccurredAt   = DateTime.UtcNow,
                Notes        = $"Bán {quantity} x {product.ProductName}"
            });
        }
    }

    public async Task ProcessExpiredItemsAsync(int storeId)
    {
        var expiredItems = await stockRepo.Queryable
            .Include(s => s.Ingredient)
            .Where(s => s.StoreId == storeId
                     && s.ExpiryDate.HasValue
                     && s.ExpiryDate.Value < DateTime.UtcNow
                     && s.CurrentStock > 0)
            .ToListAsync();

        foreach (var item in expiredItems)
        {
            await costRepo.AddAsync(new StoreCostRecord
            {
                StoreId      = storeId,
                IngredientId = item.IngredientId,
                Quantity     = item.CurrentStock,
                Cost         = item.CurrentStock * (item.Ingredient.Price ?? 0),
                CostType     = "WasteCost",
                OccurredAt   = DateTime.UtcNow,
                Notes        = $"Hàng hết hạn ngày {item.ExpiryDate:yyyy-MM-dd}"
            });

            item.CurrentStock = 0;
            item.UpdatedAt    = DateTime.UtcNow;
            await stockRepo.UpdateAsync(item);
        }
    }

    public async Task AddStockFromShipmentAsync(int shipmentId, int storeId)
    {
        var shipment = await shipmentRepo.FindAsync(shipmentId);
        if (shipment is null) return;

        var lines = await shipmentLineRepo.Queryable
            .Where(l => l.ShipmentId == shipmentId)
            .ToListAsync();

        // Gom tổng số lượng cần cộng theo ingredient để tránh update cùng 1 row nhiều lần
        var addByIngredient = new Dictionary<int, decimal>();
        var wasteByIngredient = new Dictionary<int, decimal>();

        foreach (var line in lines)
        {
            var receivedQty = line.ReceivedQuantity ?? line.ShippedQuantity ?? 0;
            var damagedQty  = line.DamagedQuantity ?? 0;
            if (receivedQty <= 0 && damagedQty <= 0) continue;

            var usableQty = receivedQty - damagedQty;
            if (usableQty < 0) usableQty = 0;

            var product = await productRepo.FindAsync(line.ProductId);
            if (product?.RecipeId is null) continue;

            var recipeIngredients = await recipeIngredientRepo.Queryable
                .Where(ri => ri.RecipeId == product.RecipeId)
                .ToListAsync();

            foreach (var ri in recipeIngredients)
            {
                if (usableQty > 0)
                {
                    var addQty = (ri.Quantity ?? 0m) * usableQty;
                    if (addQty != 0)
                    {
                        if (addByIngredient.ContainsKey(ri.IngredientId))
                            addByIngredient[ri.IngredientId] += addQty;
                        else
                            addByIngredient[ri.IngredientId] = addQty;
                    }
                }

                if (damagedQty > 0)
                {
                    var wasteQty = (ri.Quantity ?? 0m) * damagedQty;
                    if (wasteQty != 0)
                    {
                        if (wasteByIngredient.ContainsKey(ri.IngredientId))
                            wasteByIngredient[ri.IngredientId] += wasteQty;
                        else
                            wasteByIngredient[ri.IngredientId] = wasteQty;
                    }
                }
            }
        }

        if (addByIngredient.Count == 0 && wasteByIngredient.Count == 0) return;

        var now = DateTime.UtcNow;
        foreach (var (ingredientId, totalAdd) in addByIngredient)
        {
            // Query tracked entity để EF không bị conflict tracking
            var existing = await db.StoreIngredientStocks
                .FirstOrDefaultAsync(s =>
                    s.StoreId      == storeId &&
                    s.IngredientId == ingredientId &&
                    s.ExpiryDate   == shipment.ExpiryDate);

            if (existing is not null)
            {
                existing.CurrentStock += totalAdd;
                existing.UpdatedAt     = now;
            }
            else
            {
                db.StoreIngredientStocks.Add(new StoreIngredientStock
                {
                    StoreId      = storeId,
                    IngredientId = ingredientId,
                    CurrentStock = totalAdd,
                    ExpiryDate   = shipment.ExpiryDate,
                    UpdatedAt    = now
                });
            }
        }

        // Ghi nhận WasteCost cho phần hư hỏng (không cộng vào tồn kho usable)
        if (wasteByIngredient.Count > 0)
        {
            var ingIds = wasteByIngredient.Keys.ToList();
            var ingMap = await db.Ingredients
                .Where(i => ingIds.Contains(i.IngredientId))
                .ToDictionaryAsync(i => i.IngredientId, i => i);

            foreach (var (ingredientId, wasteQty) in wasteByIngredient)
            {
                if (!ingMap.TryGetValue(ingredientId, out var ing)) continue;

                db.StoreCostRecords.Add(new StoreCostRecord
                {
                    StoreId      = storeId,
                    IngredientId = ingredientId,
                    Quantity     = wasteQty,
                    Cost         = wasteQty * (ing.Price ?? 0m),
                    CostType     = "WasteCost",
                    OccurredAt   = now,
                    Notes        = $"Hàng hỏng khi nhận lô #{shipmentId}"
                });
            }
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<StoreCostRecordDto>> GetCostRecordsAsync(int storeId, string? costType)
    {
        var query = costRepo.Queryable
            .Include(c => c.Ingredient)
            .Where(c => c.StoreId == storeId);

        if (!string.IsNullOrWhiteSpace(costType))
            query = query.Where(c => c.CostType == costType);

        return await query
            .OrderByDescending(c => c.OccurredAt)
            .Select(c => new StoreCostRecordDto
            {
                Id             = c.Id,
                StoreId        = c.StoreId,
                IngredientId   = c.IngredientId,
                IngredientName = c.Ingredient.IngredientName,
                Unit           = c.Ingredient.Unit,
                Quantity       = c.Quantity,
                Cost           = c.Cost,
                CostType       = c.CostType,
                OccurredAt     = c.OccurredAt,
                Notes          = c.Notes
            })
            .ToListAsync();
    }
}
