using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models.Product;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/products")]
public class ProductsController(IProductService productService) : ApiController
{
    /// <summary>GET /api/products — Lấy danh sách sản phẩm kèm ProductTypeName.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await productService.GetAllProductsAsync());

    /// <summary>GET /api/products/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetProductByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>POST /api/products — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductModel model)
        => Ok(await productService.CreateProductAsync(model));

    /// <summary>PUT /api/products/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductModel model)
    {
        var result = await productService.UpdateProductAsync(id, model);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>DELETE /api/products/{id} — Xóa mềm (Status = Inactive). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await productService.DeleteProductAsync(id);
        return success ? NoContent() : NotFound();
    }
}
