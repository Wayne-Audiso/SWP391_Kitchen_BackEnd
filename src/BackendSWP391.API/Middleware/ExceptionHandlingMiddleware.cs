using BackendSWP391.Application.Exceptions;
using BackendSWP391.Application.Models;
using BackendSWP391.Core.Exceptions;
using Newtonsoft.Json;

namespace BackendSWP391.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, ex.Message);

        var statusCode = ex switch
        {
            NotFoundException              => StatusCodes.Status404NotFound,
            ResourceNotFoundException      => StatusCodes.Status404NotFound,
            BadRequestException            => StatusCodes.Status400BadRequest,
            UnprocessableRequestException  => StatusCodes.Status422UnprocessableEntity,
            _                              => StatusCodes.Status500InternalServerError
        };

        var errors  = new List<string> { ex.Message };
        var message = ex switch
        {
            NotFoundException             => "Không tìm thấy dữ liệu",
            ResourceNotFoundException     => "Không tìm thấy tài nguyên",
            BadRequestException           => "Dữ liệu đầu vào không hợp lệ",
            UnprocessableRequestException => "Không thể xử lý yêu cầu",
            _                             => "Đã xảy ra lỗi hệ thống"
        };

        var body = ApiResult<object>.Failure(errors, message, statusCode);
        var json = JsonConvert.SerializeObject(body, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = statusCode;

        return context.Response.WriteAsync(json);
    }
}
