using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.Product;
using BackendSWP391.Application.Services;

namespace BackendSWP391.API.Controllers;

[Route("api/product-types")]
public class ProductTypesController(IProductService productService) : ApiController
{
    /// <summary>GET /api/product-types</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await productService.GetAllProductTypesAsync();
        return Ok(ApiResult<List<ProductTypeDto>>.Ok(data, "Lấy danh sách loại sản phẩm thành công"));
    }

    /// <summary>GET /api/product-types/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetProductTypeByIdAsync(id);
        if (result is null)
            return NotFound(ApiResult<ProductTypeDto>.NotFound($"Không tìm thấy loại sản phẩm với Id = {id}"));

        return Ok(ApiResult<ProductTypeDto>.Ok(result, "Lấy thông tin loại sản phẩm thành công"));
    }

    /// <summary>POST /api/product-types — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductTypeModel model)
    {
        var result = await productService.CreateProductTypeAsync(model);
        return StatusCode(201, ApiResult<ProductTypeDto>.Created(result, "Tạo loại sản phẩm thành công"));
    }

    /// <summary>PUT /api/product-types/{id} — Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductTypeModel model)
    {
        var result = await productService.UpdateProductTypeAsync(id, model);
        if (result is null)
            return NotFound(ApiResult<ProductTypeDto>.NotFound($"Không tìm thấy loại sản phẩm với Id = {id}"));

        return Ok(ApiResult<ProductTypeDto>.Ok(result, "Cập nhật loại sản phẩm thành công"));
    }

    /// <summary>DELETE /api/product-types/{id} — Xóa cứng (không có cột Status). Yêu cầu role Admin.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await productService.DeleteProductTypeAsync(id);
        if (!success)
            return NotFound(ApiResult<bool>.NotFound($"Không tìm thấy loại sản phẩm với Id = {id}"));

        return Ok(ApiResult<bool>.Ok(true, "Xóa loại sản phẩm thành công"));
    }
}
