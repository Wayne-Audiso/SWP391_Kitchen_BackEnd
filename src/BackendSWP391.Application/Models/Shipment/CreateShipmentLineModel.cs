namespace BackendSWP391.Application.Models.Shipment;

public class CreateShipmentLineModel
{
    public int  ProductId       { get; set; }
    public int? ShippedQuantity { get; set; }
}
