using Mapster;
using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.Product;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product>          _productRepo;
    private readonly IGenericRepository<ProductType>      _productTypeRepo;
    private readonly IGenericRepository<Recipe>           _recipeRepo;
    private readonly IGenericRepository<RecipeIngredient> _recipeIngredientRepo;
    private readonly IGenericRepository<Ingredient>       _ingredientRepo;

    public ProductService(
        IGenericRepository<Product>          productRepo,
        IGenericRepository<ProductType>      productTypeRepo,
        IGenericRepository<Recipe>           recipeRepo,
        IGenericRepository<RecipeIngredient> recipeIngredientRepo,
        IGenericRepository<Ingredient>       ingredientRepo)
    {
        _productRepo          = productRepo;
        _productTypeRepo      = productTypeRepo;
        _recipeRepo           = recipeRepo;
        _recipeIngredientRepo = recipeIngredientRepo;
        _ingredientRepo       = ingredientRepo;
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
                Unit            = p.Unit,
                RecipeId        = p.RecipeId
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
                Unit            = p.Unit,
                RecipeId        = p.RecipeId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductModel model)
    {
        var entity = new Product
        {
            ProductTypeId = model.ProductTypeId,
            ProductName   = model.ProductName,
            Unit          = model.Unit,
            Status        = "Active",
            RecipeId      = model.RecipeId
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
        entity.RecipeId      = model.RecipeId;

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

    public async Task<SellProductResult> SellProductAsync(int productId, int quantity)
    {
        var product = await _productRepo.FindAsync(productId);
        if (product is null)
            return SellProductResult.Fail($"Không tìm thấy sản phẩm với Id = {productId}");

        if (product.RecipeId is null)
            return SellProductResult.Fail("Sản phẩm này chưa được liên kết với công thức nào");

        var recipeIngredients = await _recipeIngredientRepo.Queryable
            .Where(ri => ri.RecipeId == product.RecipeId)
            .ToListAsync();

        if (!recipeIngredients.Any())
            return SellProductResult.Fail("Công thức không có nguyên liệu nào");

        // Check stock for each ingredient
        var insufficient = new List<string>();
        var ingredientEntities = new List<(Ingredient ing, decimal required)>();

        foreach (var ri in recipeIngredients)
        {
            var ing = await _ingredientRepo.FindAsync(ri.IngredientId);
            if (ing is null) continue;

            var required = (ri.Quantity ?? 0) * quantity;
            var currentStock = ing.CurrentStock ?? 0;

            if (currentStock < required)
                insufficient.Add($"{ing.IngredientName}: cần {required} {ing.Unit}, còn {currentStock} {ing.Unit}");
            else
                ingredientEntities.Add((ing, required));
        }

        if (insufficient.Any())
            return SellProductResult.Fail($"Không đủ nguyên liệu: {string.Join("; ", insufficient)}");

        // Deduct stock
        foreach (var (ing, required) in ingredientEntities)
        {
            ing.CurrentStock = (ing.CurrentStock ?? 0) - required;
            await _ingredientRepo.UpdateAsync(ing);
        }

        return SellProductResult.Success();
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
