using BackendSWP391.Application.Models.Shipment;

namespace BackendSWP391.Application.Services;

public interface IShipmentService
{
    Task<List<ShipmentDto>> GetAllShipmentsAsync();
    Task<ShipmentDto?> GetShipmentByIdAsync(int id);
    Task<List<ShipmentDto>> GetShipmentsByOrderAsync(int orderId);
    Task<ShipmentDto> CreateShipmentAsync(CreateShipmentModel model);
    Task<ShipmentDto?> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusModel model);
    Task<ShipmentDto?> ReceiveShipmentAsync(int id, ReceiveShipmentModel model);
    Task<bool> DeleteShipmentAsync(int id);
}
