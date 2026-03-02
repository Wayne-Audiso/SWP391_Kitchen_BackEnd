using BackendSWP391.Application.Models.StoreOrder;

namespace BackendSWP391.Application.Services;

public interface IStoreOrderService
{
    Task<List<StoreOrderDto>> GetAllOrdersAsync();
    Task<StoreOrderDto?> GetOrderByIdAsync(int id);
    Task<List<StoreOrderDto>> GetOrdersByStoreAsync(int storeId);
    Task<StoreOrderDto> CreateOrderAsync(CreateStoreOrderModel model);
    Task<StoreOrderDto?> UpdateOrderStatusAsync(int id, UpdateStoreOrderStatusModel model);
    Task<bool> DeleteOrderAsync(int id);
}
