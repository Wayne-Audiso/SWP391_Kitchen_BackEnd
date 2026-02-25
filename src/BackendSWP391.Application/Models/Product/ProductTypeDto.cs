namespace BackendSWP391.Application.Models.Product;

public class ProductTypeDto
{
    public int     ProductTypeId    { get; set; }
    public string  TypeName         { get; set; } = default!;
    public string? Description      { get; set; }
    public string? StorageCondition { get; set; }
}
