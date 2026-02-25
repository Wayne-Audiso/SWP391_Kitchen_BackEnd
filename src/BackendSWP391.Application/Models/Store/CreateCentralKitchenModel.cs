namespace BackendSWP391.Application.Models.Store;

public class CreateCentralKitchenModel
{
    public int     CentralKitchenId { get; set; }
    public string  Name             { get; set; } = default!;
    public string? Address          { get; set; }
    public string? Phone            { get; set; }
}
