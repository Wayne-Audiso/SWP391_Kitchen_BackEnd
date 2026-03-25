using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.StoreOrder;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class StoreOrderService(
    IGenericRepository<StoreOrder>       orderRepo,
    IGenericRepository<StoreOrderLine>   lineRepo,
    IGenericRepository<Product>          productRepo,
    IGenericRepository<RecipeIngredient> recipeIngredientRepo,
    IGenericRepository<Ingredient>       ingredientRepo) : IStoreOrderService
{
    private IQueryable<StoreOrderDto> ProjectedQuery =>
        orderRepo.Queryable
            .Include(o => o.CentralKitchen)
            .Include(o => o.FranchiseStore)
            .Include(o => o.OrderLines).ThenInclude(l => l.Product)
            .Select(o => new StoreOrderDto
            {
                StoreOrderId     = o.StoreOrderId,
                CentralKitchenId = o.CentralKitchenId,
                KitchenName      = o.CentralKitchen != null ? o.CentralKitchen.Name : null,
                FranchiseStoreId = o.FranchiseStoreId,
                StoreName        = o.FranchiseStore != null ? o.FranchiseStore.StoreName : null,
                OrderDate         = o.OrderDate,
                Status            = o.Status,
                DeliveryDate      = o.DeliveryDate,
                RejectReason      = o.RejectReason,
                ProductionBatchId = o.ProductionBatchId,
                Lines            = o.OrderLines.Select(l => new StoreOrderLineDto
                {
                    StoreOrderLineId = l.StoreOrderLineId,
                    ProductId        = l.ProductId,
                    ProductName      = l.Product != null ? l.Product.ProductName : null,
                    Unit             = l.Product != null ? l.Product.Unit : null,
                    Quantity         = l.Quantity
                }).ToList()
            });

    public async Task<List<StoreOrderDto>> GetAllOrdersAsync()
        => await ProjectedQuery.ToListAsync();

    public async Task<StoreOrderDto?> GetOrderByIdAsync(int id)
        => await ProjectedQuery.FirstOrDefaultAsync(o => o.StoreOrderId == id);

    public async Task<List<StoreOrderDto>> GetOrdersByStoreAsync(int storeId)
        => await ProjectedQuery.Where(o => o.FranchiseStoreId == storeId).ToListAsync();

    public async Task<StoreOrderDto> CreateOrderAsync(CreateStoreOrderModel model)
    {
        var entity = new StoreOrder
        {
            CentralKitchenId = model.CentralKitchenId,
            FranchiseStoreId = model.FranchiseStoreId,
            Quantity         = model.Lines.Sum(l => l.Quantity), // keep for backward compat
            DeliveryDate     = model.DeliveryDate,
            OrderDate        = DateTime.UtcNow,
            Status           = "Pending"
        };

        await orderRepo.AddAsync(entity);

        foreach (var lm in model.Lines)
        {
            await lineRepo.AddAsync(new StoreOrderLine
            {
                StoreOrderId = entity.StoreOrderId,
                ProductId    = lm.ProductId,
                Quantity     = lm.Quantity
            });
        }

        return (await GetOrderByIdAsync(entity.StoreOrderId))!;
    }

    public async Task<StoreOrderDto?> UpdateOrderStatusAsync(int id, UpdateStoreOrderStatusModel model)
    {
        var entity = await orderRepo.FindAsync(id);
        if (entity is null) return null;

        entity.Status = model.Status;
        if (model.RejectReason != null)
            entity.RejectReason = model.RejectReason;
        await orderRepo.UpdateAsync(entity);
        return await GetOrderByIdAsync(id);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var entity = await orderRepo.FindAsync(id);
        if (entity is null) return false;

        entity.Status = "Inactive";
        await orderRepo.UpdateAsync(entity);
        return true;
    }

    public async Task<StockCheckResult?> CheckStockAsync(int orderId)
    {
        var order = await orderRepo.Queryable
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.StoreOrderId == orderId);

        if (order is null) return null;

        // Build required ingredient map: ingredientId → totalRequired
        var required = new Dictionary<int, decimal>();

        foreach (var line in order.OrderLines)
        {
            var product = await productRepo.Queryable
                .FirstOrDefaultAsync(p => p.ProductId == line.ProductId);

            if (product?.RecipeId == null) continue;

            var recipeIngredients = await recipeIngredientRepo.Queryable
                .Where(ri => ri.RecipeId == product.RecipeId)
                .ToListAsync();

            foreach (var ri in recipeIngredients)
            {
                var totalNeeded = (ri.Quantity ?? 0) * line.Quantity;
                if (required.ContainsKey(ri.IngredientId))
                    required[ri.IngredientId] += totalNeeded;
                else
                    required[ri.IngredientId] = totalNeeded;
            }
        }

        var shortages = new List<IngredientShortage>();

        foreach (var (ingredientId, totalRequired) in required)
        {
            var ingredient = await ingredientRepo.FindAsync(ingredientId);
            if (ingredient is null) continue;

            var available = ingredient.CurrentStock ?? 0;
            if (available < totalRequired)
            {
                shortages.Add(new IngredientShortage
                {
                    IngredientId   = ingredientId,
                    IngredientName = ingredient.IngredientName,
                    Required       = totalRequired,
                    Available      = available,
                    Unit           = ingredient.Unit
                });
            }
        }

        return new StockCheckResult
        {
            Sufficient = shortages.Count == 0,
            Shortages  = shortages
        };
    }
}
