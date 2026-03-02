namespace BackendSWP391.Application.Models.Ingredient;

public class UpdateIngredientModel
{
    public string   IngredientName   { get; set; } = default!;
    public string?  Unit             { get; set; }
    public string?  StorageCondition { get; set; }
    public int?     MinStock         { get; set; }
    public decimal? Price            { get; set; }
}
