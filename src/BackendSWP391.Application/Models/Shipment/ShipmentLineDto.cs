namespace BackendSWP391.Application.Models.Shipment;

public class ShipmentLineDto
{
    public int    ShipmentLineId    { get; set; }
    public int    ProductId         { get; set; }
    public string? ProductName      { get; set; }
    public int?   ShippedQuantity   { get; set; }
    public int?   ReceivedQuantity  { get; set; }
    public int?   DamagedQuantity   { get; set; }
}
