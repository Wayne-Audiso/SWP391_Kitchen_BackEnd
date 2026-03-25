using BackendSWP391.Application.Models.ProductionBatch;

namespace BackendSWP391.Application.Services;

public interface IProductionBatchService
{
    Task<List<ProductionBatchDto>> GetAllBatchesAsync();
    Task<ProductionBatchDto?> GetBatchByIdAsync(int id);
    Task<ProductionBatchDto> CreateBatchAsync(CreateProductionBatchModel model);
    Task<ProductionBatchDto?> UpdateBatchStatusAsync(int id, UpdateProductionBatchStatusModel model);
    Task<bool> DeleteBatchAsync(int id);
}
