namespace BackendSWP391.Application.Models.Shipment;

public class ShipmentDto
{
    public int       ShipmentId       { get; set; }
    public int       StoreOrderId     { get; set; }
    public int       CentralKitchenId { get; set; }
    public string?   KitchenName      { get; set; }
    public DateTime? ShipmentDate     { get; set; }
    public string?   DeliveryStatus   { get; set; }
    public DateTime? ReceivedDate     { get; set; }
    public List<ShipmentLineDto> Lines { get; set; } = new();
}
