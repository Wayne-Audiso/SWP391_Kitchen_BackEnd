namespace BackendSWP391.Application.Models.StoreInventory;

public class StoreCostRecordDto
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; }
    public string Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal Cost { get; set; }
    public string CostType { get; set; }
    public DateTime OccurredAt { get; set; }
    public string Notes { get; set; }
}
