namespace BackendSWP391.Application.Models.Shipment;

public class CreateShipmentModel
{
    public int  StoreOrderId     { get; set; }
    public int  CentralKitchenId { get; set; }
    public List<CreateShipmentLineModel> Lines { get; set; } = new();
}
