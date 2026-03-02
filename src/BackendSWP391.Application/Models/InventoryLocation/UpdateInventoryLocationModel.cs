namespace BackendSWP391.Application.Models.InventoryLocation;

public class UpdateInventoryLocationModel
{
    public string  Name         { get; set; } = default!;
    public string? LocationType { get; set; }
    public string? Status       { get; set; }
}
