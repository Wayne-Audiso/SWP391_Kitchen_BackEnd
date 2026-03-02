namespace BackendSWP391.Application.Models.Store;

public class FranchiseStoreDto
{
    public int     StoreId     { get; set; }
    public int     KitchenId   { get; set; }
    public string? KitchenName { get; set; }   // từ Kitchen.Name (Include)
    public string  StoreName   { get; set; } = default!;
    public string? Address     { get; set; }
}
