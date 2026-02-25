namespace BackendSWP391.Application.Models.Product;

public class CreateProductModel
{
    public int     ProductId     { get; set; }
    public int     ProductTypeId { get; set; }
    public string  ProductName   { get; set; } = default!;
    public string? Unit          { get; set; }
}
