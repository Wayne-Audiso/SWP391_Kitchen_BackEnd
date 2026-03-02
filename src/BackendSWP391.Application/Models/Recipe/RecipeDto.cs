namespace BackendSWP391.Application.Models.Recipe;

public class RecipeDto
{
    public int                     RecipeId    { get; set; }
    public string                  RecipeName  { get; set; } = default!;
    public string?                 Description { get; set; }
    public DateTime?               CreatedDate { get; set; }
    public decimal                 TotalCost   { get; set; }
    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
}
