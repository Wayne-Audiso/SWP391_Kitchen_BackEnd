namespace BackendSWP391.Application.Models;

/// <summary>
/// Wrapper thống nhất cho mọi HTTP response trong hệ thống.
/// FE luôn nhận được cấu trúc JSON nhất quán dù thành công hay thất bại.
/// </summary>
public class ApiResult<T>
{
    private ApiResult() { }

    /// <summary>Trạng thái xử lý: true = thành công, false = thất bại.</summary>
    public bool Success { get; init; }

    /// <summary>Thông điệp mô tả kết quả (ví dụ: "Lấy dữ liệu thành công").</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Dữ liệu trả về (null khi có lỗi).</summary>
    public T? Data { get; init; }

    /// <summary>HTTP Status Code được nhúng vào body để FE tiện xử lý.</summary>
    public int StatusCode { get; init; }

    /// <summary>Danh sách lỗi chi tiết (validation errors, exception messages, …).</summary>
    public List<string> Errors { get; init; } = new();

    /// <summary>
    /// Metadata tuỳ chọn — dùng cho phân trang hoặc thông tin bổ sung.
    /// Ví dụ: new { TotalCount = 100, PageSize = 10, CurrentPage = 1 }
    /// </summary>
    public object? Metadata { get; init; }

    // ── Factory methods ──────────────────────────────────────────────────────

    /// <summary>200 OK — Trả về dữ liệu thành công.</summary>
    public static ApiResult<T> Ok(T data, string message = "Thành công")
        => new() { Success = true, Message = message, Data = data, StatusCode = 200 };

    /// <summary>201 Created — Tạo mới tài nguyên thành công.</summary>
    public static ApiResult<T> Created(T data, string message = "Tạo mới thành công")
        => new() { Success = true, Message = message, Data = data, StatusCode = 201 };

    /// <summary>400 Bad Request — Dữ liệu đầu vào không hợp lệ.</summary>
    public static ApiResult<T> BadRequest(string message, IEnumerable<string>? errors = null)
        => new()
        {
            Success    = false,
            Message    = message,
            StatusCode = 400,
            Errors     = errors is null ? new List<string>() : new List<string>(errors)
        };

    /// <summary>404 Not Found — Không tìm thấy tài nguyên.</summary>
    public static ApiResult<T> NotFound(string message = "Không tìm thấy dữ liệu")
        => new() { Success = false, Message = message, StatusCode = 404 };

    /// <summary>
    /// Lỗi hệ thống (5xx) hoặc lỗi tuỳ chỉnh — dùng chủ yếu bởi ExceptionHandlingMiddleware.
    /// </summary>
    /// <param name="errors">Danh sách message lỗi chi tiết.</param>
    /// <param name="message">Thông điệp tổng quát.</param>
    /// <param name="statusCode">HTTP status code (mặc định 500).</param>
    public static ApiResult<T> Failure(
        IEnumerable<string> errors,
        string message    = "Đã xảy ra lỗi hệ thống",
        int    statusCode = 500)
        => new()
        {
            Success    = false,
            Message    = message,
            StatusCode = statusCode,
            Errors     = new List<string>(errors)
        };

    // ── Fluent helper ────────────────────────────────────────────────────────

    /// <summary>
    /// Gắn thêm metadata phân trang. Gọi sau khi tạo ApiResult.
    /// <code>
    /// ApiResult&lt;List&lt;T&gt;&gt;.Ok(items).WithMetadata(new { TotalCount = 100, PageSize = 10, CurrentPage = 1 })
    /// </code>
    /// </summary>
    public ApiResult<T> WithMetadata(object metadata)
        => new()
        {
            Success    = Success,
            Message    = Message,
            Data       = Data,
            StatusCode = StatusCode,
            Errors     = Errors,
            Metadata   = metadata
        };
}
