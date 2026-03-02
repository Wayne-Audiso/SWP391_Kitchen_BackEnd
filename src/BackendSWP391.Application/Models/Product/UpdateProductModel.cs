namespace BackendSWP391.Application.Models.Product;

public class UpdateProductModel
{
    public int     ProductTypeId { get; set; }
    public string  ProductName   { get; set; } = default!;
    public string? Status        { get; set; }
    public string? Unit          { get; set; }
}
