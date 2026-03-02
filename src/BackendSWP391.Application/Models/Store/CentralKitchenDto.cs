namespace BackendSWP391.Application.Models.Store;

public class CentralKitchenDto
{
    public int       CentralKitchenId { get; set; }
    public string    Name             { get; set; } = default!;
    public string?   Address          { get; set; }
    public string?   Phone            { get; set; }
    public string?   Status           { get; set; }
    public DateTime? CreatedAt        { get; set; }
    public DateTime? UpdatedAt        { get; set; }
}
