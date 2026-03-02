namespace BackendSWP391.Application.Models.InventoryLocation;

public class CreateInventoryLocationModel
{
    public int    CentralKitchenId { get; set; }
    public string Name             { get; set; } = default!;
    public string? LocationType    { get; set; }
}
