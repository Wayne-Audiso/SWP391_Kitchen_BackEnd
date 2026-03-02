using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.InventoryLocation;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/inventory-locations")]
public class InventoryLocationsController(IInventoryLocationService locationService) : ApiController
{
    /// <summary>GET /api/inventory-locations — Lấy danh sách vị trí kho kèm tên bếp.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await locationService.GetAllLocationsAsync();
        return Ok(ApiResult<List<InventoryLocationDto>>.Ok(data, "Lấy danh sách vị trí kho thành công"));
    }

    /// <summary>GET /api/inventory-locations/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await locationService.GetLocationByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<InventoryLocationDto>.NotFound($"Không tìm thấy vị trí kho với Id = {id}"));
        return Ok(ApiResult<InventoryLocationDto>.Ok(result, "Lấy thông tin vị trí kho thành công"));
    }

    /// <summary>GET /api/inventory-locations/by-kitchen/{kitchenId} — Lọc theo Bếp trung tâm.</summary>
    [HttpGet("by-kitchen/{kitchenId:int}")]
    public async Task<IActionResult> GetByKitchen(int kitchenId)
    {
        var data = await locationService.GetLocationsByKitchenAsync(kitchenId);
        return Ok(ApiResult<List<InventoryLocationDto>>.Ok(data, "Lấy vị trí kho theo bếp thành công"));
    }

    /// <summary>POST /api/inventory-locations — Yêu cầu role Admin hoặc Manager.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryLocationModel model)
    {
        var result = await locationService.CreateLocationAsync(model);
        return StatusCode(201, ApiResult<InventoryLocationDto>.Created(result, "Tạo vị trí kho thành công"));
    }

    /// <summary>PUT /api/inventory-locations/{id} — Yêu cầu role Admin hoặc Manager.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryLocationModel model)
    {
        var result = await locationService.UpdateLocationAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<InventoryLocationDto>.NotFound($"Không tìm thấy vị trí kho với Id = {id}"));
        return Ok(ApiResult<InventoryLocationDto>.Ok(result, "Cập nhật vị trí kho thành công"));
    }

    /// <summary>DELETE /api/inventory-locations/{id} — Xóa mềm (Status = Inactive). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await locationService.DeleteLocationAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy vị trí kho với Id = {id}"));
        return Ok(ApiResult<bool>.Ok(true, "Xóa vị trí kho thành công"));
    }
}
