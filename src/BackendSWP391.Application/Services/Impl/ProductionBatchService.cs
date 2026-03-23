using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.ProductionBatch;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class ProductionBatchService(
    IGenericRepository<ProductionBatch>     batchRepo,
    IGenericRepository<ProductionBatchLine> lineRepo,
    IGenericRepository<StoreOrder>          orderRepo) : IProductionBatchService
{
    private IQueryable<ProductionBatchDto> ProjectedQuery =>
        batchRepo.Queryable
            .Include(b => b.CentralKitchen)
            .Include(b => b.Lines).ThenInclude(l => l.Product)
            .Include(b => b.StoreOrders)
            .Select(b => new ProductionBatchDto
            {
                ProductionBatchId = b.ProductionBatchId,
                CentralKitchenId  = b.CentralKitchenId,
                KitchenName       = b.CentralKitchen != null ? b.CentralKitchen.Name : null,
                Status            = b.Status,
                CreatedDate       = b.CreatedDate,
                CompletedDate     = b.CompletedDate,
                Notes             = b.Notes,
                Lines             = b.Lines.Select(l => new ProductionBatchLineDto
                {
                    ProductionBatchLineId = l.ProductionBatchLineId,
                    ProductId             = l.ProductId,
                    ProductName           = l.Product != null ? l.Product.ProductName : null,
                    RequiredQuantity      = l.RequiredQuantity,
                    ProducedQuantity      = l.ProducedQuantity
                }).ToList(),
                OrderIds = b.StoreOrders.Select(o => o.StoreOrderId).ToList()
            });

    public async Task<List<ProductionBatchDto>> GetAllBatchesAsync()
        => await ProjectedQuery.ToListAsync();

    public async Task<ProductionBatchDto?> GetBatchByIdAsync(int id)
        => await ProjectedQuery.FirstOrDefaultAsync(b => b.ProductionBatchId == id);

    public async Task<ProductionBatchDto> CreateBatchAsync(CreateProductionBatchModel model)
    {
        var entity = new ProductionBatch
        {
            CentralKitchenId = model.CentralKitchenId,
            Status           = "PendingApproval",
            CreatedDate      = DateTime.UtcNow,
            Notes            = model.Notes
        };

        await batchRepo.AddAsync(entity);

        // Add lines
        foreach (var lm in model.Lines)
        {
            await lineRepo.AddAsync(new ProductionBatchLine
            {
                ProductionBatchId = entity.ProductionBatchId,
                ProductId         = lm.ProductId,
                RequiredQuantity  = lm.RequiredQuantity
            });
        }

        // Link orders to this batch
        foreach (var orderId in model.OrderIds)
        {
            var order = await orderRepo.FindAsync(orderId);
            if (order != null)
            {
                order.ProductionBatchId = entity.ProductionBatchId;
                order.Status            = "InProduction";
                await orderRepo.UpdateAsync(order);
            }
        }

        return (await GetBatchByIdAsync(entity.ProductionBatchId))!;
    }

    public async Task<ProductionBatchDto?> UpdateBatchStatusAsync(int id, UpdateProductionBatchStatusModel model)
    {
        var entity = await batchRepo.FindAsync(id);
        if (entity is null) return null;

        entity.Status = model.Status;

        // When production completes → auto-advance all linked orders to Delivering
        if (model.Status == "ProductionCompleted")
        {
            entity.CompletedDate = DateTime.UtcNow;

            var linkedOrders = await orderRepo.Queryable
                .Where(o => o.ProductionBatchId == id)
                .ToListAsync();

            foreach (var order in linkedOrders)
            {
                order.Status = "Delivering";
                await orderRepo.UpdateAsync(order);
            }
        }

        await batchRepo.UpdateAsync(entity);
        return await GetBatchByIdAsync(id);
    }

    public async Task<bool> DeleteBatchAsync(int id)
    {
        var entity = await batchRepo.FindAsync(id);
        if (entity is null) return false;

        await batchRepo.DeleteAsync(entity);
        return true;
    }
}
