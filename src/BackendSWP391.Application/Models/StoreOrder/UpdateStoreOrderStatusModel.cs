namespace BackendSWP391.Application.Models.StoreOrder;

/// <summary>
/// Luồng trạng thái: Pending → Approved / Rejected → InProduction → InDelivery → Completed
/// </summary>
public class UpdateStoreOrderStatusModel
{
    public string Status { get; set; } = default!;
}
