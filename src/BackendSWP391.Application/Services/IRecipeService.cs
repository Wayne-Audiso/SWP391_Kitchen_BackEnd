using BackendSWP391.Application.Models.Recipe;

namespace BackendSWP391.Application.Services;

public interface IRecipeService
{
    Task<List<RecipeDto>> GetAllRecipesAsync();
    Task<RecipeDto?> GetRecipeByIdAsync(int id);
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeModel model);
    Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipeModel model);
    Task<bool> DeleteRecipeAsync(int id);
}
