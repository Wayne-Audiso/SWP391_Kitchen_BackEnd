namespace BackendSWP391.Application.Models.Product;

public class SellProductResult
{
    public bool   IsSuccess { get; private set; }
    public string Message   { get; private set; } = default!;

    public static SellProductResult Success(string message = "Bán hàng thành công")
        => new() { IsSuccess = true, Message = message };

    public static SellProductResult Fail(string message)
        => new() { IsSuccess = false, Message = message };
}
