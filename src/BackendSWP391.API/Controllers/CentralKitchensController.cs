using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.Store;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/central-kitchens")]
public class CentralKitchensController(IStoreService storeService) : ApiController
{
    /// <summary>GET /api/central-kitchens</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await storeService.GetAllKitchensAsync());

    /// <summary>GET /api/central-kitchens/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await storeService.GetKitchenByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>POST /api/central-kitchens — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCentralKitchenModel model)
        => Ok(await storeService.CreateKitchenAsync(model));

    /// <summary>PUT /api/central-kitchens/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCentralKitchenModel model)
    {
        var result = await storeService.UpdateKitchenAsync(id, model);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>DELETE /api/central-kitchens/{id} — Xóa mềm (Status = Inactive). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await storeService.DeleteKitchenAsync(id);
        return success ? NoContent() : NotFound();
    }
}
