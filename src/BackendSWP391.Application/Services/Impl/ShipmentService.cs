using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Exceptions;
using BackendSWP391.Application.Models.Shipment;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Persistence;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class ShipmentService(
    IGenericRepository<Shipment>     shipmentRepo,
    IGenericRepository<ShipmentLine> lineRepo,
    IGenericRepository<StoreOrder>   storeOrderRepo,
    IStoreInventoryService           storeInventoryService,
    IGenericRepository<Product>          productRepo,
    IGenericRepository<RecipeIngredient> recipeIngredientRepo,
    IGenericRepository<Ingredient>       ingredientRepo,
    DatabaseContext                     db) : IShipmentService
{
    private IQueryable<ShipmentDto> ProjectedQuery =>
        shipmentRepo.Queryable
            .Include(s => s.CentralKitchen)
            .Include(s => s.ShipmentLines)
                .ThenInclude(l => l.Product)
            .Select(s => new ShipmentDto
            {
                ShipmentId       = s.ShipmentId,
                StoreOrderId     = s.StoreOrderId,
                CentralKitchenId = s.CentralKitchenId,
                KitchenName      = s.CentralKitchen != null ? s.CentralKitchen.Name : null,
                ShipmentDate      = s.ShipmentDate,
                DeliveryStatus    = s.DeliveryStatus,
                ReceivedDate      = s.ReceivedDate,
                ManufacturingDate = s.ManufacturingDate,
                ExpiryDate        = s.ExpiryDate,
                Lines             = s.ShipmentLines.Select(l => new ShipmentLineDto
                {
                    ShipmentLineId   = l.ShipmentLineId,
                    ProductId        = l.ProductId,
                    ProductName      = l.Product != null ? l.Product.ProductName : null,
                    ShippedQuantity  = l.ShippedQuantity,
                    ReceivedQuantity = l.ReceivedQuantity,
                    DamagedQuantity  = l.DamagedQuantity
                }).ToList()
            });

    public async Task<List<ShipmentDto>> GetAllShipmentsAsync()
        => await ProjectedQuery.ToListAsync();

    public async Task<ShipmentDto?> GetShipmentByIdAsync(int id)
        => await ProjectedQuery.FirstOrDefaultAsync(s => s.ShipmentId == id);

    public async Task<List<ShipmentDto>> GetShipmentsByOrderAsync(int orderId)
        => await ProjectedQuery.Where(s => s.StoreOrderId == orderId).ToListAsync();

    public async Task<ShipmentDto> CreateShipmentAsync(CreateShipmentModel model)
    {
        var entity = new Shipment
        {
            StoreOrderId     = model.StoreOrderId,
            CentralKitchenId = model.CentralKitchenId,
            ShipmentDate     = DateTime.UtcNow,
            DeliveryStatus   = "Preparing"
        };

        await shipmentRepo.AddAsync(entity);

        foreach (var lineModel in model.Lines)
        {
            var line = new ShipmentLine
            {
                ShipmentId      = entity.ShipmentId,
                ProductId       = lineModel.ProductId,
                ShippedQuantity = lineModel.ShippedQuantity
            };
            await lineRepo.AddAsync(line);
        }

        return (await GetShipmentByIdAsync(entity.ShipmentId))!;
    }

    public async Task<ShipmentDto?> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusModel model)
    {
        var entity = await shipmentRepo.FindAsync(id);
        if (entity is null) return null;

        // Nếu bắt đầu giao (InDelivery) thì trừ tồn kho bếp trung tâm theo recipe * shippedQuantity
        var previousStatus = entity.DeliveryStatus;
        if (!string.Equals(previousStatus, "InDelivery", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(model.DeliveryStatus, "InDelivery", StringComparison.OrdinalIgnoreCase))
        {
            await DeductCentralStockForShipmentAsync(id);
        }

        entity.DeliveryStatus = model.DeliveryStatus;

        if (model.ManufacturingDate.HasValue)
        {
            entity.ManufacturingDate = model.ManufacturingDate;
            entity.ExpiryDate        = model.ManufacturingDate.Value.AddDays(10);
        }

        await shipmentRepo.UpdateAsync(entity);
        return await GetShipmentByIdAsync(id);
    }

    public async Task<ShipmentDto?> ReceiveShipmentAsync(int id, ReceiveShipmentModel model)
    {
        var entity = await shipmentRepo.FindAsync(id);
        if (entity is null) return null;

        foreach (var lineModel in model.Lines)
        {
            var line = await lineRepo.FindAsync(lineModel.ShipmentLineId);
            if (line is null || line.ShipmentId != id) continue;

            var shipped  = line.ShippedQuantity ?? 0;
            var received = lineModel.ReceivedQuantity ?? shipped;
            var damaged  = lineModel.DamagedQuantity ?? 0;

            if (received > shipped)
                throw new BadRequestException($"Số lượng nhận ({received}) không thể vượt quá số lượng giao ({shipped})");

            if (damaged > received)
                throw new BadRequestException($"Số lượng hỏng ({damaged}) không thể vượt quá số lượng nhận ({received})");

            line.ReceivedQuantity = lineModel.ReceivedQuantity;
            line.DamagedQuantity  = lineModel.DamagedQuantity;
            await lineRepo.UpdateAsync(line);
        }

        entity.ReceivedDate   = DateTime.UtcNow;
        entity.DeliveryStatus = "Delivered";
        await shipmentRepo.UpdateAsync(entity);

        // Cập nhật kho Franchise Store khi xác nhận nhận hàng
        var order = await storeOrderRepo.FindAsync(entity.StoreOrderId);
        if (order is not null)
            await storeInventoryService.AddStockFromShipmentAsync(id, order.FranchiseStoreId);

        // Đồng bộ trạng thái đơn hàng: nếu tất cả shipment của đơn đã Delivered/Cancelled thì order → Delivered
        if (order is not null)
        {
            var allDone = await shipmentRepo.Queryable
                .Where(s => s.StoreOrderId == order.StoreOrderId)
                // Tránh StringComparison (EF không translate). DB collation thường đã case-insensitive.
                .AllAsync(s => s.DeliveryStatus == "Delivered" || s.DeliveryStatus == "Cancelled");

            if (allDone && string.Equals(order.Status, "Delivering", StringComparison.OrdinalIgnoreCase))
            {
                order.Status = "Delivered";
                await storeOrderRepo.UpdateAsync(order);
            }
        }

        return await GetShipmentByIdAsync(id);
    }

    public async Task<bool> DeleteShipmentAsync(int id)
    {
        var entity = await shipmentRepo.FindAsync(id);
        if (entity is null) return false;

        // Nếu shipment đang giao (đã trừ kho central) mà bị hủy thì hoàn kho central
        if (string.Equals(entity.DeliveryStatus, "InDelivery", StringComparison.OrdinalIgnoreCase))
        {
            await RestoreCentralStockForShipmentAsync(entity.ShipmentId);
        }

        entity.DeliveryStatus = "Cancelled";
        await shipmentRepo.UpdateAsync(entity);
        return true;
    }

    private async Task DeductCentralStockForShipmentAsync(int shipmentId)
    {
        // Transaction đã được mở ở TransactionMiddleware (mỗi request).
        var shipment = await db.Shipments
            .Include(s => s.ShipmentLines)
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment is null)
        {
            return;
        }

        // Aggregate required ingredients
        var requiredByIngredient = new Dictionary<int, decimal>();

        foreach (var line in shipment.ShipmentLines)
        {
            var shippedQty = line.ShippedQuantity ?? 0;
            if (shippedQty <= 0) continue;

            var product = await productRepo.FindAsync(line.ProductId);
            if (product?.RecipeId is null) continue;

            var ris = await recipeIngredientRepo.Queryable
                .Where(ri => ri.RecipeId == product.RecipeId)
                .ToListAsync();

            foreach (var ri in ris)
            {
                var need = (ri.Quantity ?? 0m) * shippedQty;
                if (need == 0) continue;

                if (requiredByIngredient.ContainsKey(ri.IngredientId))
                    requiredByIngredient[ri.IngredientId] += need;
                else
                    requiredByIngredient[ri.IngredientId] = need;
            }
        }

        // Check + deduct
        foreach (var (ingredientId, need) in requiredByIngredient)
        {
            var ing = await ingredientRepo.FindAsync(ingredientId);
            if (ing is null) continue;

            var available = ing.CurrentStock ?? 0m;
            if (available < need)
                throw new BadRequestException($"Kho bếp trung tâm không đủ '{ing.IngredientName}': cần {need} {ing.Unit}, còn {available} {ing.Unit}");
        }

        foreach (var (ingredientId, need) in requiredByIngredient)
        {
            var ing = await ingredientRepo.FindAsync(ingredientId);
            if (ing is null) continue;

            ing.CurrentStock = (ing.CurrentStock ?? 0m) - need;
            db.Ingredients.Update(ing);
        }

        await db.SaveChangesAsync();
    }

    private async Task RestoreCentralStockForShipmentAsync(int shipmentId)
    {
        var shipment = await db.Shipments
            .Include(s => s.ShipmentLines)
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment is null) return;

        // Tính lại lượng nguyên liệu đã trừ theo shippedQuantity (vì cancel trước khi store nhận)
        var restoreByIngredient = new Dictionary<int, decimal>();

        foreach (var line in shipment.ShipmentLines)
        {
            var shippedQty = line.ShippedQuantity ?? 0;
            if (shippedQty <= 0) continue;

            var product = await productRepo.FindAsync(line.ProductId);
            if (product?.RecipeId is null) continue;

            var ris = await recipeIngredientRepo.Queryable
                .Where(ri => ri.RecipeId == product.RecipeId)
                .ToListAsync();

            foreach (var ri in ris)
            {
                var qty = (ri.Quantity ?? 0m) * shippedQty;
                if (qty == 0) continue;

                if (restoreByIngredient.ContainsKey(ri.IngredientId))
                    restoreByIngredient[ri.IngredientId] += qty;
                else
                    restoreByIngredient[ri.IngredientId] = qty;
            }
        }

        foreach (var (ingredientId, qty) in restoreByIngredient)
        {
            var ing = await ingredientRepo.FindAsync(ingredientId);
            if (ing is null) continue;

            ing.CurrentStock = (ing.CurrentStock ?? 0m) + qty;
            db.Ingredients.Update(ing);
        }

        await db.SaveChangesAsync();
    }
}
