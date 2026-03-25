namespace BackendSWP391.Application.Models.StoreOrder;

public class StockCheckResult
{
    public bool                   Sufficient { get; set; }
    public List<IngredientShortage> Shortages { get; set; } = new();
}

public class IngredientShortage
{
    public int     IngredientId   { get; set; }
    public string  IngredientName { get; set; } = default!;
    public decimal Required       { get; set; }
    public decimal Available      { get; set; }
    public string? Unit           { get; set; }
}
