using BackendSWP391.Application.Models.InventoryLocation;

namespace BackendSWP391.Application.Services;

public interface IInventoryLocationService
{
    Task<List<InventoryLocationDto>> GetAllLocationsAsync();
    Task<InventoryLocationDto?> GetLocationByIdAsync(int id);
    Task<List<InventoryLocationDto>> GetLocationsByKitchenAsync(int kitchenId);
    Task<InventoryLocationDto> CreateLocationAsync(CreateInventoryLocationModel model);
    Task<InventoryLocationDto?> UpdateLocationAsync(int id, UpdateInventoryLocationModel model);
    Task<bool> DeleteLocationAsync(int id);
}
