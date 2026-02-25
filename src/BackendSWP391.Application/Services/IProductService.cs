using BackendSWP391.Application.Models.Product;

namespace BackendSWP391.Application.Services;

public interface IProductService
{
    // ── Product ──────────────────────────────────────────────────────────────
    Task<List<ProductDto>>  GetAllProductsAsync();
    Task<ProductDto?>       GetProductByIdAsync(int id);
    Task<ProductDto>        CreateProductAsync(CreateProductModel model);
    Task<ProductDto?>       UpdateProductAsync(int id, UpdateProductModel model);
    /// <summary>Xóa mềm: cập nhật Status = "Inactive".</summary>
    Task<bool>              DeleteProductAsync(int id);

    // ── ProductType ──────────────────────────────────────────────────────────
    Task<List<ProductTypeDto>>  GetAllProductTypesAsync();
    Task<ProductTypeDto?>       GetProductTypeByIdAsync(int id);
    Task<ProductTypeDto>        CreateProductTypeAsync(CreateProductTypeModel model);
    Task<ProductTypeDto?>       UpdateProductTypeAsync(int id, UpdateProductTypeModel model);
    /// <summary>Xóa cứng (ProductType không có cột Status).</summary>
    Task<bool>                  DeleteProductTypeAsync(int id);
}
