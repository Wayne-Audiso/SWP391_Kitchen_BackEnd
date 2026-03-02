using Microsoft.EntityFrameworkCore;
using BackendSWP391.Application.Models.Recipe;
using BackendSWP391.Core.Models;
using BackendSWP391.DataAccess.Repositories;

namespace BackendSWP391.Application.Services.Impl;

public class RecipeService(
    IGenericRepository<Recipe> recipeRepo,
    IGenericRepository<RecipeIngredient> riRepo) : IRecipeService
{
    private IQueryable<RecipeDto> ProjectedQuery =>
        recipeRepo.Queryable
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Select(r => new RecipeDto
            {
                RecipeId    = r.RecipeId,
                RecipeName  = r.RecipeName,
                Description = r.Description,
                CreatedDate = r.CreatedDate,
                TotalCost   = r.RecipeIngredients.Sum(ri =>
                    (ri.Quantity ?? 0) * (ri.Ingredient.Price ?? 0)),
                Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    RecipeIngredientId = ri.RecipeIngredientId,
                    IngredientId       = ri.IngredientId,
                    IngredientName     = ri.Ingredient.IngredientName,
                    Unit               = ri.Ingredient.Unit,
                    Price              = ri.Ingredient.Price,
                    Quantity           = ri.Quantity,
                    TotalCost          = (ri.Quantity ?? 0) * (ri.Ingredient.Price ?? 0)
                }).ToList()
            });

    public async Task<List<RecipeDto>> GetAllRecipesAsync()
        => await ProjectedQuery.ToListAsync();

    public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
        => await ProjectedQuery.FirstOrDefaultAsync(r => r.RecipeId == id);

    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeModel model)
    {
        var entity = new Recipe
        {
            RecipeName  = model.RecipeName,
            Description = model.Description,
            CreatedDate = DateTime.UtcNow
        };

        await recipeRepo.AddAsync(entity);

        foreach (var line in model.Ingredients)
        {
            await riRepo.AddAsync(new RecipeIngredient
            {
                RecipeId     = entity.RecipeId,
                IngredientId = line.IngredientId,
                Quantity     = line.Quantity
            });
        }

        return await ProjectedQuery.FirstAsync(r => r.RecipeId == entity.RecipeId);
    }

    public async Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipeModel model)
    {
        var entity = await recipeRepo.FindAsync(id);
        if (entity is null) return null;

        entity.RecipeName  = model.RecipeName;
        entity.Description = model.Description;
        await recipeRepo.UpdateAsync(entity);

        // Replace ingredient lines: delete old, insert new
        var oldLines = await riRepo.Queryable
            .Where(ri => ri.RecipeId == id)
            .ToListAsync();
        foreach (var old in oldLines)
            await riRepo.DeleteAsync(old);

        foreach (var line in model.Ingredients)
        {
            await riRepo.AddAsync(new RecipeIngredient
            {
                RecipeId     = id,
                IngredientId = line.IngredientId,
                Quantity     = line.Quantity
            });
        }

        return await ProjectedQuery.FirstOrDefaultAsync(r => r.RecipeId == id);
    }

    public async Task<bool> DeleteRecipeAsync(int id)
    {
        var entity = await recipeRepo.FindAsync(id);
        if (entity is null) return false;

        await recipeRepo.DeleteAsync(entity);
        return true;
    }
}
