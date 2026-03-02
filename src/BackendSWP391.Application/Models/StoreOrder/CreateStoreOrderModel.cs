namespace BackendSWP391.Application.Models.StoreOrder;

public class CreateStoreOrderModel
{
    public int       CentralKitchenId { get; set; }
    public int       FranchiseStoreId { get; set; }
    public int?      Quantity         { get; set; }
    public DateTime? DeliveryDate     { get; set; }
}
