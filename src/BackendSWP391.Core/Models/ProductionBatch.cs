using System;
using System.Collections.Generic;

namespace BackendSWP391.Core.Models;

public class ProductionBatch
{
    public int ProductionBatchId { get; set; }

    public int CentralKitchenId { get; set; }

    /// <summary>PendingApproval → Approved → InProducing → ProductionCompleted</summary>
    public string Status { get; set; } = "PendingApproval";

    public DateTime? CreatedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string? Notes { get; set; }

    public virtual CentralKitchen CentralKitchen { get; set; } = null!;

    public virtual ICollection<ProductionBatchLine> Lines { get; set; } = new List<ProductionBatchLine>();

    public virtual ICollection<StoreOrder> StoreOrders { get; set; } = new List<StoreOrder>();
}
