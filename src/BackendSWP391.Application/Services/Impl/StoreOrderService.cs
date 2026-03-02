using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.StoreOrder;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class StoreOrderService(
    IGenericRepository<StoreOrder> orderRepo) : IStoreOrderService
{
    private IQueryable<StoreOrderDto> ProjectedQuery =>
        orderRepo.Queryable
            .Include(o => o.CentralKitchen)
            .Include(o => o.FranchiseStore)
            .Select(o => new StoreOrderDto
            {
                StoreOrderId     = o.StoreOrderId,
                CentralKitchenId = o.CentralKitchenId,
                KitchenName      = o.CentralKitchen != null ? o.CentralKitchen.Name : null,
                FranchiseStoreId = o.FranchiseStoreId,
                StoreName        = o.FranchiseStore != null ? o.FranchiseStore.StoreName : null,
                OrderDate        = o.OrderDate,
                Status           = o.Status,
                Quantity         = o.Quantity,
                DeliveryDate     = o.DeliveryDate
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
            Quantity         = model.Quantity,
            DeliveryDate     = model.DeliveryDate,
            OrderDate        = DateTime.UtcNow,
            Status           = "Pending"
        };

        await orderRepo.AddAsync(entity);
        return (await GetOrderByIdAsync(entity.StoreOrderId))!;
    }

    public async Task<StoreOrderDto?> UpdateOrderStatusAsync(int id, UpdateStoreOrderStatusModel model)
    {
        var entity = await orderRepo.FindAsync(id);
        if (entity is null) return null;

        entity.Status = model.Status;
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
}
