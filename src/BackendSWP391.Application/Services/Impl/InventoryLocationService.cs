using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.InventoryLocation;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class InventoryLocationService(
    IGenericRepository<InventoryLocation> locationRepo) : IInventoryLocationService
{
    private IQueryable<InventoryLocationDto> ProjectedQuery =>
        locationRepo.Queryable
            .Include(l => l.CentralKitchen)
            .Select(l => new InventoryLocationDto
            {
                InventoryLocationId = l.InventoryLocationId,
                CentralKitchenId    = l.CentralKitchenId,
                KitchenName         = l.CentralKitchen != null ? l.CentralKitchen.Name : null,
                Name                = l.Name,
                LocationType        = l.LocationType,
                Status              = l.Status,
                UpdatedAt           = l.UpdatedAt
            });

    public async Task<List<InventoryLocationDto>> GetAllLocationsAsync()
        => await ProjectedQuery.ToListAsync();

    public async Task<InventoryLocationDto?> GetLocationByIdAsync(int id)
        => await ProjectedQuery.FirstOrDefaultAsync(l => l.InventoryLocationId == id);

    public async Task<List<InventoryLocationDto>> GetLocationsByKitchenAsync(int kitchenId)
        => await ProjectedQuery.Where(l => l.CentralKitchenId == kitchenId).ToListAsync();

    public async Task<InventoryLocationDto> CreateLocationAsync(CreateInventoryLocationModel model)
    {
        var entity = new InventoryLocation
        {
            CentralKitchenId = model.CentralKitchenId,
            Name             = model.Name,
            LocationType     = model.LocationType,
            Status           = "Active",
            UpdatedAt        = DateTime.UtcNow
        };

        await locationRepo.AddAsync(entity);
        return (await GetLocationByIdAsync(entity.InventoryLocationId))!;
    }

    public async Task<InventoryLocationDto?> UpdateLocationAsync(int id, UpdateInventoryLocationModel model)
    {
        var entity = await locationRepo.FindAsync(id);
        if (entity is null) return null;

        entity.Name         = model.Name;
        entity.LocationType = model.LocationType;
        entity.Status       = model.Status;
        entity.UpdatedAt    = DateTime.UtcNow;

        await locationRepo.UpdateAsync(entity);
        return await GetLocationByIdAsync(id);
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        var entity = await locationRepo.FindAsync(id);
        if (entity is null) return false;

        entity.Status    = "Inactive";
        entity.UpdatedAt = DateTime.UtcNow;
        await locationRepo.UpdateAsync(entity);
        return true;
    }
}
