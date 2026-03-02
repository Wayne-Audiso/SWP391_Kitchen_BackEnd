namespace BackendSWP391.Application.Models.Shipment;

public class ReceiveShipmentModel
{
    public List<ReceiveShipmentLineModel> Lines { get; set; } = new();
}
