namespace BackendSWP391.Application.Models.Recipe;

public class RecipeIngredientDto
{
    public int      RecipeIngredientId { get; set; }
    public int      IngredientId       { get; set; }
    public string   IngredientName     { get; set; } = default!;
    public string?  Unit               { get; set; }
    public decimal? Price              { get; set; }
    public decimal? Quantity           { get; set; }
    public decimal? TotalCost          { get; set; }
}
