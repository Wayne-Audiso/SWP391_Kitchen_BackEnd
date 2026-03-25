namespace BackendSWP391.Application.Models.ProductionBatch;

public class CreateProductionBatchModel
{
    public int           CentralKitchenId { get; set; }
    public List<int>     OrderIds         { get; set; } = new();
    public List<CreateProductionBatchLineModel> Lines { get; set; } = new();
    public string?       Notes            { get; set; }
}

public class CreateProductionBatchLineModel
{
    public int ProductId         { get; set; }
    public int RequiredQuantity  { get; set; }
}
