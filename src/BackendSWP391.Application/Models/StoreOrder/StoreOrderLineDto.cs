namespace BackendSWP391.Application.Models.StoreOrder;

public class StoreOrderLineDto
{
    public int     StoreOrderLineId { get; set; }
    public int     ProductId        { get; set; }
    public string? ProductName      { get; set; }
    public string? Unit             { get; set; }
    public int     Quantity         { get; set; }
}
