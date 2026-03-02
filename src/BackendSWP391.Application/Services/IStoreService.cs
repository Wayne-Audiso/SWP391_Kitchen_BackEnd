using BackendSWP391.Application.Models.Store;

namespace BackendSWP391.Application.Services;

public interface IStoreService
{
    // ── CentralKitchen ───────────────────────────────────────────────────────
    Task<List<CentralKitchenDto>>  GetAllKitchensAsync();
    Task<CentralKitchenDto?>       GetKitchenByIdAsync(int id);
    Task<CentralKitchenDto>        CreateKitchenAsync(CreateCentralKitchenModel model);
    Task<CentralKitchenDto?>       UpdateKitchenAsync(int id, UpdateCentralKitchenModel model);
    /// <summary>Xóa mềm: cập nhật Status = "Inactive".</summary>
    Task<bool>                     DeleteKitchenAsync(int id);

    // ── FranchiseStore ───────────────────────────────────────────────────────
    Task<List<FranchiseStoreDto>>  GetAllStoresAsync();
    Task<FranchiseStoreDto?>       GetStoreByIdAsync(int id);
    Task<FranchiseStoreDto>        CreateStoreAsync(CreateFranchiseStoreModel model);
    Task<FranchiseStoreDto?>       UpdateStoreAsync(int id, UpdateFranchiseStoreModel model);
    /// <summary>Xóa cứng (FranchiseStore không có cột Status).</summary>
    Task<bool>                     DeleteStoreAsync(int id);
}
