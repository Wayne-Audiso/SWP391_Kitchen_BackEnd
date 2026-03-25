namespace BackendSWP391.Application.Models.StoreOrder;

public class StoreOrderDto
{
    public int       StoreOrderId      { get; set; }
    public int       CentralKitchenId  { get; set; }
    public string?   KitchenName       { get; set; }
    public int       FranchiseStoreId  { get; set; }
    public string?   StoreName         { get; set; }
    public DateTime? OrderDate         { get; set; }
    public string?   Status            { get; set; }
    public DateTime? DeliveryDate      { get; set; }
    public string?   RejectReason     { get; set; }
    public int?      ProductionBatchId { get; set; }
    public List<StoreOrderLineDto> Lines { get; set; } = new();
    public int TotalQuantity => Lines.Sum(l => l.Quantity);
}
