using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.ProductionBatch;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/production-batches")]
public class ProductionBatchesController(IProductionBatchService batchService) : ApiController
{
    /// <summary>GET /api/production-batches — Danh sách lô sản xuất.</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator,Central Kitchen Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await batchService.GetAllBatchesAsync();
        return Ok(ApiResult<List<ProductionBatchDto>>.Ok(data, "Lấy danh sách lô sản xuất thành công"));
    }

    /// <summary>GET /api/production-batches/{id}</summary>
    [Authorize(Roles = "Admin,Manager,Supply Coordinator,Central Kitchen Staff")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await batchService.GetBatchByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<ProductionBatchDto>.NotFound($"Không tìm thấy lô sản xuất với Id = {id}"));
        return Ok(ApiResult<ProductionBatchDto>.Ok(result, "Lấy thông tin lô sản xuất thành công"));
    }

    /// <summary>POST /api/production-batches — SC tạo lô sản xuất từ các đơn NeedsProduction.</summary>
    [Authorize(Roles = "Admin,Supply Coordinator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionBatchModel model)
    {
        var result = await batchService.CreateBatchAsync(model);
        return StatusCode(201, ApiResult<ProductionBatchDto>.Created(result, "Tạo lô sản xuất thành công"));
    }

    /// <summary>
    /// PUT /api/production-batches/{id}/status — Cập nhật trạng thái lô sản xuất.
    /// PendingApproval → Approved → InProducing → ProductionCompleted
    /// Khi ProductionCompleted: tự động chuyển đơn hàng liên kết → Delivering
    /// </summary>
    [Authorize(Roles = "Admin,Supply Coordinator,Central Kitchen Staff")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateProductionBatchStatusModel model)
    {
        var result = await batchService.UpdateBatchStatusAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<ProductionBatchDto>.NotFound($"Không tìm thấy lô sản xuất với Id = {id}"));
        return Ok(ApiResult<ProductionBatchDto>.Ok(result, "Cập nhật trạng thái lô sản xuất thành công"));
    }

    /// <summary>DELETE /api/production-batches/{id} — Xóa lô sản xuất.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await batchService.DeleteBatchAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy lô sản xuất với Id = {id}"));
        return Ok(ApiResult<bool>.Ok(true, "Xóa lô sản xuất thành công"));
    }
}
