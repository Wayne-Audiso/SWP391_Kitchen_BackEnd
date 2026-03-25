namespace BackendSWP391.Application.Models.ProductionBatch;

public class UpdateProductionBatchStatusModel
{
    /// <summary>PendingApproval → Approved → InProducing → ProductionCompleted</summary>
    public string Status { get; set; } = default!;
}
