using CR.Constants.ErrorCodes;
using CR.DtoBase;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using CR.ApplicationBase.Localization;

namespace CR.Core.API.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Chuyển đổi Result thành IActionResult kèm theo Message được map tự động từ ErrorCode
    /// </summary>
    public static IActionResult ToActionResult(this Result result, ControllerBase controller, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(ApiResponse<object>.Ok(null!, successMessage ?? "Thao tác thành công."));
        }

        var localization = controller.HttpContext.RequestServices.GetRequiredService<ILocalization>();
        string errorMessage = localization.Localize($"error_{result.ErrorCode}");
        return controller.BadRequest(ApiResponse<object>.Fail(errorMessage, result.ErrorCode));
    }

    /// <summary>
    /// Chuyển đổi Result<T> thành IActionResult kèm theo Message được map tự động từ ErrorCode
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(ApiResponse<T>.Ok(result.Value, successMessage ?? "Thao tác thành công."));
        }

        var localization = controller.HttpContext.RequestServices.GetRequiredService<ILocalization>();
        string errorMessage = localization.Localize($"error_{result.ErrorCode}");
        return controller.BadRequest(ApiResponse<T>.Fail(errorMessage, result.ErrorCode));
    }
}
