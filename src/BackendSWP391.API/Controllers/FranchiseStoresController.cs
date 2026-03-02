using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Store;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/franchise-stores")]
public class FranchiseStoresController(IStoreService storeService) : ApiController
{
    /// <summary>GET /api/franchise-stores — Lấy danh sách cửa hàng kèm KitchenName.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await storeService.GetAllStoresAsync();
        return Ok(ApiResult<List<FranchiseStoreDto>>.Ok(data, "Lấy danh sách cửa hàng nhượng quyền thành công"));
    }

    /// <summary>GET /api/franchise-stores/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await storeService.GetStoreByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<FranchiseStoreDto>.NotFound($"Không tìm thấy cửa hàng với Id = {id}"));

        return Ok(ApiResult<FranchiseStoreDto>.Ok(result, "Lấy thông tin cửa hàng thành công"));
    }

    /// <summary>POST /api/franchise-stores — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFranchiseStoreModel model)
    {
        var result = await storeService.CreateStoreAsync(model);
        return StatusCode(201, ApiResult<FranchiseStoreDto>.Created(result, "Tạo cửa hàng nhượng quyền thành công"));
    }

    /// <summary>PUT /api/franchise-stores/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFranchiseStoreModel model)
    {
        var result = await storeService.UpdateStoreAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<FranchiseStoreDto>.NotFound($"Không tìm thấy cửa hàng với Id = {id}"));

        return Ok(ApiResult<FranchiseStoreDto>.Ok(result, "Cập nhật cửa hàng thành công"));
    }

    /// <summary>DELETE /api/franchise-stores/{id} — Xóa cứng (FranchiseStore không có Status). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await storeService.DeleteStoreAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy cửa hàng với Id = {id}"));

        return Ok(ApiResult<bool>.Ok(true, "Xóa cửa hàng thành công"));
    }
}
