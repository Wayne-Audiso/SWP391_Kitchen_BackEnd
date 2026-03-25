namespace BackendSWP391.Application.Models.StoreInventory;

public class StoreStockDto
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; }
    public string Unit { get; set; }
    public decimal CurrentStock { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? MinStock { get; set; }
    public decimal? Price { get; set; }
}

public class SellAtStoreModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
