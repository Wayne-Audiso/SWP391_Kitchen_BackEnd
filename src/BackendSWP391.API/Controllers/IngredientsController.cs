using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Ingredient;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/ingredients")]
public class IngredientsController(IIngredientService ingredientService) : ApiController
{
    /// <summary>GET /api/ingredients — Lấy danh sách nguyên liệu.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await ingredientService.GetAllIngredientsAsync();
        return Ok(ApiResult<List<IngredientDto>>.Ok(data, "Lấy danh sách nguyên liệu thành công"));
    }

    /// <summary>GET /api/ingredients/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await ingredientService.GetIngredientByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<IngredientDto>.NotFound($"Không tìm thấy nguyên liệu với Id = {id}"));
        return Ok(ApiResult<IngredientDto>.Ok(result, "Lấy thông tin nguyên liệu thành công"));
    }

    /// <summary>POST /api/ingredients — Yêu cầu role Admin hoặc Manager.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIngredientModel model)
    {
        var result = await ingredientService.CreateIngredientAsync(model);
        return StatusCode(201, ApiResult<IngredientDto>.Created(result, "Tạo nguyên liệu thành công"));
    }

    /// <summary>PUT /api/ingredients/{id} — Yêu cầu role Admin hoặc Manager.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIngredientModel model)
    {
        var result = await ingredientService.UpdateIngredientAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<IngredientDto>.NotFound($"Không tìm thấy nguyên liệu với Id = {id}"));
        return Ok(ApiResult<IngredientDto>.Ok(result, "Cập nhật nguyên liệu thành công"));
    }

    /// <summary>DELETE /api/ingredients/{id} — Xóa cứng. Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await ingredientService.DeleteIngredientAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy nguyên liệu với Id = {id}"));
        return Ok(ApiResult<bool>.Ok(true, "Xóa nguyên liệu thành công"));
    }
}
