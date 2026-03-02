using Mapster;
using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.Store;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class StoreService : IStoreService
{
    private readonly IGenericRepository<CentralKitchen> _kitchenRepo;
    private readonly IGenericRepository<FranchiseStore> _storeRepo;

    public StoreService(
        IGenericRepository<CentralKitchen> kitchenRepo,
        IGenericRepository<FranchiseStore> storeRepo)
    {
        _kitchenRepo = kitchenRepo;
        _storeRepo   = storeRepo;
    }

    // ── CentralKitchen ───────────────────────────────────────────────────────

    public async Task<List<CentralKitchenDto>> GetAllKitchensAsync()
    {
        var list = await _kitchenRepo.Queryable.ToListAsync();
        return list.Adapt<List<CentralKitchenDto>>();
    }

    public async Task<CentralKitchenDto?> GetKitchenByIdAsync(int id)
    {
        var entity = await _kitchenRepo.FindAsync(id);
        return entity?.Adapt<CentralKitchenDto>();
    }

    public async Task<CentralKitchenDto> CreateKitchenAsync(CreateCentralKitchenModel model)
    {
        var entity = new CentralKitchen
        {
            Name             = model.Name,
            Address          = model.Address,
            Phone            = model.Phone,
            Status           = "Active",
            CreatedAt        = DateTime.UtcNow,
            UpdatedAt        = DateTime.UtcNow
        };

        await _kitchenRepo.AddAsync(entity);
        return entity.Adapt<CentralKitchenDto>();
    }

    public async Task<CentralKitchenDto?> UpdateKitchenAsync(int id, UpdateCentralKitchenModel model)
    {
        var entity = await _kitchenRepo.FindAsync(id);
        if (entity is null) return null;

        entity.Name      = model.Name;
        entity.Address   = model.Address;
        entity.Phone     = model.Phone;
        entity.Status    = model.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _kitchenRepo.UpdateAsync(entity);
        return entity.Adapt<CentralKitchenDto>();
    }

    public async Task<bool> DeleteKitchenAsync(int id)
    {
        var entity = await _kitchenRepo.FindAsync(id);
        if (entity is null) return false;

        entity.Status    = "Inactive";
        entity.UpdatedAt = DateTime.UtcNow;
        await _kitchenRepo.UpdateAsync(entity);
        return true;
    }

    // ── FranchiseStore ───────────────────────────────────────────────────────

    public async Task<List<FranchiseStoreDto>> GetAllStoresAsync()
    {
        // Include Kitchen để lấy KitchenName, dùng LINQ projection tránh circular reference
        return await _storeRepo.Queryable
            .Include(s => s.Kitchen)
            .Select(s => new FranchiseStoreDto
            {
                StoreId     = s.StoreId,
                KitchenId   = s.KitchenId,
                KitchenName = s.Kitchen != null ? s.Kitchen.Name : null,
                StoreName   = s.StoreName,
                Address     = s.Address
            })
            .ToListAsync();
    }

    public async Task<FranchiseStoreDto?> GetStoreByIdAsync(int id)
    {
        return await _storeRepo.Queryable
            .Include(s => s.Kitchen)
            .Where(s => s.StoreId == id)
            .Select(s => new FranchiseStoreDto
            {
                StoreId     = s.StoreId,
                KitchenId   = s.KitchenId,
                KitchenName = s.Kitchen != null ? s.Kitchen.Name : null,
                StoreName   = s.StoreName,
                Address     = s.Address
            })
            .FirstOrDefaultAsync();
    }

    public async Task<FranchiseStoreDto> CreateStoreAsync(CreateFranchiseStoreModel model)
    {
        var entity = new FranchiseStore
        {
            KitchenId = model.KitchenId,
            StoreName = model.StoreName,
            Address   = model.Address
        };

        await _storeRepo.AddAsync(entity);

        // Trả về đầy đủ thông tin (có KitchenName)
        return (await GetStoreByIdAsync(entity.StoreId))!;
    }

    public async Task<FranchiseStoreDto?> UpdateStoreAsync(int id, UpdateFranchiseStoreModel model)
    {
        var entity = await _storeRepo.FindAsync(id);
        if (entity is null) return null;

        entity.KitchenId = model.KitchenId;
        entity.StoreName = model.StoreName;
        entity.Address   = model.Address;

        await _storeRepo.UpdateAsync(entity);

        return await GetStoreByIdAsync(id);
    }

    public async Task<bool> DeleteStoreAsync(int id)
    {
        var entity = await _storeRepo.FindAsync(id);
        if (entity is null) return false;

        await _storeRepo.DeleteAsync(entity);
        return true;
    }
}
