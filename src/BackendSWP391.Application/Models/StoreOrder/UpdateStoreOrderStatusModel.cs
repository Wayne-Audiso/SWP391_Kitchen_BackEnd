namespace BackendSWP391.Application.Models.StoreOrder;

/// <summary>
/// Luồng trạng thái mới: Pending → Submitted → Approved/Rejected → Delivering/NeedsProduction
///   → (qua ProductionBatch) InProduction → ProductionCompleted → Delivering
///   → Delivered / RejectedByStore
/// </summary>
public class UpdateStoreOrderStatusModel
{
    public string  Status       { get; set; } = default!;
    public string? RejectReason { get; set; }
}
