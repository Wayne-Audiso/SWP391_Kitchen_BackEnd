using Mapster;
using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.Product;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product>     _productRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;

    public ProductService(
        IGenericRepository<Product>     productRepo,
        IGenericRepository<ProductType> productTypeRepo)
    {
        _productRepo     = productRepo;
        _productTypeRepo = productTypeRepo;
    }

    // ── Product ──────────────────────────────────────────────────────────────

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        // Include ProductType để lấy TypeName, dùng LINQ projection tránh circular reference
        return await _productRepo.Queryable
            .Include(p => p.ProductType)
            .Select(p => new ProductDto
            {
                ProductId       = p.ProductId,
                ProductTypeId   = p.ProductTypeId,
                ProductTypeName = p.ProductType != null ? p.ProductType.TypeName : null,
                ProductName     = p.ProductName,
                Status          = p.Status,
                Unit            = p.Unit
            })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        return await _productRepo.Queryable
            .Include(p => p.ProductType)
            .Where(p => p.ProductId == id)
            .Select(p => new ProductDto
            {
                ProductId       = p.ProductId,
                ProductTypeId   = p.ProductTypeId,
                ProductTypeName = p.ProductType != null ? p.ProductType.TypeName : null,
                ProductName     = p.ProductName,
                Status          = p.Status,
                Unit            = p.Unit
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductModel model)
    {
        var entity = new Product
        {
            ProductId     = model.ProductId,
            ProductTypeId = model.ProductTypeId,
            ProductName   = model.ProductName,
            Unit          = model.Unit,
            Status        = "Active"
        };

        await _productRepo.AddAsync(entity);

        // Trả về đầy đủ thông tin (có ProductTypeName) bằng cách query lại theo ID
        return (await GetProductByIdAsync(entity.ProductId))!;
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductModel model)
    {
        var entity = await _productRepo.FindAsync(id);
        if (entity is null) return null;

        entity.ProductTypeId = model.ProductTypeId;
        entity.ProductName   = model.ProductName;
        entity.Unit          = model.Unit;
        entity.Status        = model.Status;

        await _productRepo.UpdateAsync(entity);

        return await GetProductByIdAsync(id);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var entity = await _productRepo.FindAsync(id);
        if (entity is null) return false;

        entity.Status = "Inactive";
        await _productRepo.UpdateAsync(entity);
        return true;
    }

    // ── ProductType ──────────────────────────────────────────────────────────

    public async Task<List<ProductTypeDto>> GetAllProductTypesAsync()
    {
        var list = await _productTypeRepo.Queryable.ToListAsync();
        return list.Adapt<List<ProductTypeDto>>();
    }

    public async Task<ProductTypeDto?> GetProductTypeByIdAsync(int id)
    {
        var entity = await _productTypeRepo.FindAsync(id);
        return entity?.Adapt<ProductTypeDto>();
    }

    public async Task<ProductTypeDto> CreateProductTypeAsync(CreateProductTypeModel model)
    {
        var entity = new ProductType
        {
            ProductTypeId    = model.ProductTypeId,
            TypeName         = model.TypeName,
            Description      = model.Description,
            StorageCondition = model.StorageCondition
        };

        await _productTypeRepo.AddAsync(entity);
        return entity.Adapt<ProductTypeDto>();
    }

    public async Task<ProductTypeDto?> UpdateProductTypeAsync(int id, UpdateProductTypeModel model)
    {
        var entity = await _productTypeRepo.FindAsync(id);
        if (entity is null) return null;

        entity.TypeName         = model.TypeName;
        entity.Description      = model.Description;
        entity.StorageCondition = model.StorageCondition;

        await _productTypeRepo.UpdateAsync(entity);
        return entity.Adapt<ProductTypeDto>();
    }

    public async Task<bool> DeleteProductTypeAsync(int id)
    {
        var entity = await _productTypeRepo.FindAsync(id);
        if (entity is null) return false;

        await _productTypeRepo.DeleteAsync(entity);
        return true;
    }
}
