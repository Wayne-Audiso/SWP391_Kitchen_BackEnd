using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Recipe;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/recipes")]
public class RecipesController(IRecipeService recipeService) : ApiController
{
    /// <summary>GET /api/recipes — Lấy danh sách công thức.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await recipeService.GetAllRecipesAsync();
        return Ok(ApiResult<List<RecipeDto>>.Ok(data, "Lấy danh sách công thức thành công"));
    }

    /// <summary>GET /api/recipes/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await recipeService.GetRecipeByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<RecipeDto>.NotFound($"Không tìm thấy công thức với Id = {id}"));
        return Ok(ApiResult<RecipeDto>.Ok(result, "Lấy thông tin công thức thành công"));
    }

    /// <summary>POST /api/recipes — Yêu cầu role Admin, Manager hoặc Central Kitchen Staff.</summary>
    [Authorize(Roles = "Admin,Manager,Central Kitchen Staff")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecipeModel model)
    {
        var result = await recipeService.CreateRecipeAsync(model);
        return StatusCode(201, ApiResult<RecipeDto>.Created(result, "Tạo công thức thành công"));
    }

    /// <summary>PUT /api/recipes/{id} — Yêu cầu role Admin, Manager hoặc Central Kitchen Staff.</summary>
    [Authorize(Roles = "Admin,Manager,Central Kitchen Staff")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipeModel model)
    {
        var result = await recipeService.UpdateRecipeAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<RecipeDto>.NotFound($"Không tìm thấy công thức với Id = {id}"));
        return Ok(ApiResult<RecipeDto>.Ok(result, "Cập nhật công thức thành công"));
    }

    /// <summary>DELETE /api/recipes/{id} — Xóa cứng. Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await recipeService.DeleteRecipeAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy công thức với Id = {id}"));
        return Ok(ApiResult<bool>.Ok(true, "Xóa công thức thành công"));
    }
}
