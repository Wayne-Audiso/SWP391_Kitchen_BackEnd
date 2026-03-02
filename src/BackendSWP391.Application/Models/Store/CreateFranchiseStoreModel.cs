namespace BackendSWP391.Application.Models.Store;

public class CreateFranchiseStoreModel
{
    public int     KitchenId { get; set; }
    public string  StoreName { get; set; } = default!;
    public string? Address   { get; set; }
}
