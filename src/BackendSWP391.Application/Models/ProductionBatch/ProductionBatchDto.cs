namespace BackendSWP391.Application.Models.ProductionBatch;

public class ProductionBatchDto
{
    public int       ProductionBatchId { get; set; }
    public int       CentralKitchenId  { get; set; }
    public string?   KitchenName       { get; set; }
    /// <summary>PendingApproval | Approved | InProducing | ProductionCompleted</summary>
    public string    Status            { get; set; } = default!;
    public DateTime? CreatedDate       { get; set; }
    public DateTime? CompletedDate     { get; set; }
    public string?   Notes             { get; set; }
    public List<ProductionBatchLineDto> Lines    { get; set; } = new();
    public List<int>                   OrderIds { get; set; } = new();
}

public class ProductionBatchLineDto
{
    public int    ProductionBatchLineId { get; set; }
    public int    ProductId             { get; set; }
    public string? ProductName          { get; set; }
    public int    RequiredQuantity      { get; set; }
    public int?   ProducedQuantity      { get; set; }
}
