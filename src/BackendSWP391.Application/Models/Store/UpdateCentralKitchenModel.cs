namespace BackendSWP391.Application.Models.Store;

public class UpdateCentralKitchenModel
{
    public string  Name    { get; set; } = default!;
    public string? Address { get; set; }
    public string? Phone   { get; set; }
    public string? Status  { get; set; }
}
