using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.Store;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/franchise-stores")]
public class FranchiseStoresController(IStoreService storeService) : ApiController
{
    /// <summary>GET /api/franchise-stores — Lấy danh sách cửa hàng kèm KitchenName.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await storeService.GetAllStoresAsync());

    /// <summary>GET /api/franchise-stores/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await storeService.GetStoreByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>POST /api/franchise-stores — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFranchiseStoreModel model)
        => Ok(await storeService.CreateStoreAsync(model));

    /// <summary>PUT /api/franchise-stores/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFranchiseStoreModel model)
    {
        var result = await storeService.UpdateStoreAsync(id, model);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>DELETE /api/franchise-stores/{id} — Xóa cứng (FranchiseStore không có Status). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await storeService.DeleteStoreAsync(id);
        return success ? NoContent() : NotFound();
    }
}
