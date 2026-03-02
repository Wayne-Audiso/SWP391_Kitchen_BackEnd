namespace BackendSWP391.Application.Models.InventoryLocation;

public class InventoryLocationDto
{
    public int       InventoryLocationId { get; set; }
    public int       CentralKitchenId    { get; set; }
    public string?   KitchenName         { get; set; }
    public string    Name                { get; set; } = default!;
    public string?   LocationType        { get; set; }
    public string?   Status              { get; set; }
    public DateTime? UpdatedAt           { get; set; }
}
