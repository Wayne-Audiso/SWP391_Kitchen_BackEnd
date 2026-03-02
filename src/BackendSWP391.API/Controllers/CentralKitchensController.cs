using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Store;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/central-kitchens")]
public class CentralKitchensController(IStoreService storeService) : ApiController
{
    /// <summary>GET /api/central-kitchens</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await storeService.GetAllKitchensAsync();
        return Ok(ApiResult<List<CentralKitchenDto>>.Ok(data, "Lấy danh sách bếp trung tâm thành công"));
    }

    /// <summary>GET /api/central-kitchens/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await storeService.GetKitchenByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<CentralKitchenDto>.NotFound($"Không tìm thấy bếp trung tâm với Id = {id}"));

        return Ok(ApiResult<CentralKitchenDto>.Ok(result, "Lấy thông tin bếp trung tâm thành công"));
    }

    /// <summary>POST /api/central-kitchens — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCentralKitchenModel model)
    {
        var result = await storeService.CreateKitchenAsync(model);
        return StatusCode(201, ApiResult<CentralKitchenDto>.Created(result, "Tạo bếp trung tâm thành công"));
    }

    /// <summary>PUT /api/central-kitchens/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCentralKitchenModel model)
    {
        var result = await storeService.UpdateKitchenAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<CentralKitchenDto>.NotFound($"Không tìm thấy bếp trung tâm với Id = {id}"));

        return Ok(ApiResult<CentralKitchenDto>.Ok(result, "Cập nhật bếp trung tâm thành công"));
    }

    /// <summary>DELETE /api/central-kitchens/{id} — Xóa mềm (Status = Inactive). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await storeService.DeleteKitchenAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy bếp trung tâm với Id = {id}"));

        return Ok(ApiResult<bool>.Ok(true, "Xóa bếp trung tâm thành công"));
    }
}
