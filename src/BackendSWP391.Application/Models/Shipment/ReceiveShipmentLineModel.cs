namespace BackendSWP391.Application.Models.Shipment;

public class ReceiveShipmentLineModel
{
    public int  ShipmentLineId   { get; set; }
    public int? ReceivedQuantity { get; set; }
    public int? DamagedQuantity  { get; set; }
}
