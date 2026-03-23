namespace BackendSWP391.Core.Models;

public class ProductionBatchLine
{
    public int ProductionBatchLineId { get; set; }

    public int ProductionBatchId { get; set; }

    public int ProductId { get; set; }

    public int RequiredQuantity { get; set; }

    public int? ProducedQuantity { get; set; }

    public virtual ProductionBatch ProductionBatch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
