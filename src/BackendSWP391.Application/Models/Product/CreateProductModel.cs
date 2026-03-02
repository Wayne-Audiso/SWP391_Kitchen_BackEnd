namespace BackendSWP391.Application.Models.Product;

public class CreateProductModel
{
    public int     ProductTypeId { get; set; }
    public string  ProductName   { get; set; } = default!;
    public string? Unit          { get; set; }
}
