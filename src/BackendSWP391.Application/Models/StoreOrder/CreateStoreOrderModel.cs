namespace BackendSWP391.Application.Models.StoreOrder;

public class CreateStoreOrderModel
{
    public int       CentralKitchenId { get; set; }
    public int       FranchiseStoreId { get; set; }
    public DateTime? DeliveryDate     { get; set; }
    public List<CreateStoreOrderLineModel> Lines { get; set; } = new();
}
