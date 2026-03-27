// Library/CR.WebAPIBase/Middlewares/GlobalExceptionMiddleware.cs
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CR.WebAPIBase.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (KeyNotFoundException ex)
        {
            ctx.Response.StatusCode = 404;
            await ctx.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message, 404));
        }
        catch (UnauthorizedAccessException ex)
        {
            ctx.Response.StatusCode = 401;
            await ctx.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message, 401));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception at {Path}", ctx.Request.Path);
            ctx.Response.StatusCode = 500;
            await ctx.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail("Lỗi hệ thống, vui lòng thử lại", 500));
        }
    }
}