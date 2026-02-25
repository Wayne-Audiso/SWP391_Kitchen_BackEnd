using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.Product;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/product-types")]
public class ProductTypesController(IProductService productService) : ApiController
{
    /// <summary>GET /api/product-types</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await productService.GetAllProductTypesAsync());

    /// <summary>GET /api/product-types/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetProductTypeByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>POST /api/product-types — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductTypeModel model)
        => Ok(await productService.CreateProductTypeAsync(model));

    /// <summary>PUT /api/product-types/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductTypeModel model)
    {
        var result = await productService.UpdateProductTypeAsync(id, model);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>DELETE /api/product-types/{id} — Xóa cứng (không có cột Status). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await productService.DeleteProductTypeAsync(id);
        return success ? NoContent() : NotFound();
    }
}
