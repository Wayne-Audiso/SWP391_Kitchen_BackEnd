using BackendSWP391.Application.Models.Ingredient;

namespace BackendSWP391.Application.Services;

public interface IIngredientService
{
    Task<List<IngredientDto>> GetAllIngredientsAsync();
    Task<IngredientDto?> GetIngredientByIdAsync(int id);
    Task<IngredientDto> CreateIngredientAsync(CreateIngredientModel model);
    Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientModel model);
    Task<bool> DeleteIngredientAsync(int id);
}
