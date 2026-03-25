namespace BackendSWP391.Application.Models.Shipment;

public class UpdateShipmentStatusModel
{
    public string    DeliveryStatus    { get; set; } = default!;
    public DateTime? ManufacturingDate { get; set; }
}
