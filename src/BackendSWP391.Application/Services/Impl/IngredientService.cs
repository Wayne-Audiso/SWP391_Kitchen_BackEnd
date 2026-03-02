using Mapster;
using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.Ingredient;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class IngredientService(IGenericRepository<Ingredient> ingredientRepo) : IIngredientService
{
    public async Task<List<IngredientDto>> GetAllIngredientsAsync()
    {
        var list = await ingredientRepo.Queryable.ToListAsync();
        return list.Adapt<List<IngredientDto>>();
    }

    public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
    {
        var entity = await ingredientRepo.FindAsync(id);
        return entity?.Adapt<IngredientDto>();
    }

    public async Task<IngredientDto> CreateIngredientAsync(CreateIngredientModel model)
    {
        var entity = new Ingredient
        {
            IngredientName   = model.IngredientName,
            Unit             = model.Unit,
            StorageCondition = model.StorageCondition,
            MinStock         = model.MinStock,
            Price            = model.Price
        };

        await ingredientRepo.AddAsync(entity);
        return entity.Adapt<IngredientDto>();
    }

    public async Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientModel model)
    {
        var entity = await ingredientRepo.FindAsync(id);
        if (entity is null) return null;

        entity.IngredientName   = model.IngredientName;
        entity.Unit             = model.Unit;
        entity.StorageCondition = model.StorageCondition;
        entity.MinStock         = model.MinStock;
        entity.Price            = model.Price;

        await ingredientRepo.UpdateAsync(entity);
        return entity.Adapt<IngredientDto>();
    }

    public async Task<bool> DeleteIngredientAsync(int id)
    {
        var entity = await ingredientRepo.FindAsync(id);
        if (entity is null) return false;

        await ingredientRepo.DeleteAsync(entity);
        return true;
    }
}
