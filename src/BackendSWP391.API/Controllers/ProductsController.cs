using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Product;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/products")]
public class ProductsController(IProductService productService) : ApiController
{
    /// <summary>GET /api/products — Lấy danh sách sản phẩm kèm ProductTypeName.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await productService.GetAllProductsAsync();
        return Ok(ApiResult<List<ProductDto>>.Ok(data, "Lấy danh sách sản phẩm thành công"));
    }

    /// <summary>GET /api/products/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetProductByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<ProductDto>.NotFound($"Không tìm thấy sản phẩm với Id = {id}"));

        return Ok(ApiResult<ProductDto>.Ok(result, "Lấy thông tin sản phẩm thành công"));
    }

    /// <summary>POST /api/products — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductModel model)
    {
        var result = await productService.CreateProductAsync(model);
        return StatusCode(201, ApiResult<ProductDto>.Created(result, "Tạo sản phẩm thành công"));
    }

    /// <summary>PUT /api/products/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductModel model)
    {
        var result = await productService.UpdateProductAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<ProductDto>.NotFound($"Không tìm thấy sản phẩm với Id = {id}"));

        return Ok(ApiResult<ProductDto>.Ok(result, "Cập nhật sản phẩm thành công"));
    }

    /// <summary>DELETE /api/products/{id} — Xóa mềm (Status = Inactive). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await productService.DeleteProductAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy sản phẩm với Id = {id}"));

        return Ok(ApiResult<bool>.Ok(true, "Xóa sản phẩm thành công"));
    }
}
