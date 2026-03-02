namespace BackendSWP391.Application.Models.Recipe;

public class UpdateRecipeModel
{
    public string  RecipeName   { get; set; } = default!;
    public string? Description  { get; set; }
    public List<CreateRecipeIngredientModel> Ingredients { get; set; } = new();
}
