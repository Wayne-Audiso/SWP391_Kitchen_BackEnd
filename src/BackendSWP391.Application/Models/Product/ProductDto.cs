namespace BackendSWP391.Application.Models.Product;

public class ProductDto
{
    public int     ProductId       { get; set; }
    public int     ProductTypeId   { get; set; }
    public string? ProductTypeName { get; set; }   // từ ProductType.TypeName (Include)
    public string  ProductName     { get; set; } = default!;
    public string? Status          { get; set; }
    public string? Unit            { get; set; }
}
