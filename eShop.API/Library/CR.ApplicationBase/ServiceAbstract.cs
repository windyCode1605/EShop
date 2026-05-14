using CR.ApplicationBase.Localization;
using CR.ConstantBase.Common;
using CR.Constants.ErrorCodes;
using CR.DtoBase;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CR.ApplicationBase;

public abstract class ServiceAbstract
{
    protected readonly ILogger _logger;
    protected readonly ILocalization _localization;
    protected readonly IHttpContextAccessor _httpContext;

    protected ServiceAbstract(ILogger logger)
    {
        _logger = logger;
        _httpContext = default!;
        _localization = default!;
    }

    protected ServiceAbstract(ILogger logger, IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _httpContext = httpContext;
        _localization =
            httpContext.HttpContext?.RequestServices.GetRequiredService<LocalizationBase>()
            ?? default!;

        if (
            _httpContext
                .HttpContext?.Request
                .Headers.TryGetValue(HeaderNames.XRequestId, out var requestId) == true
        )
        {
            _logger.LogInformation(
                "{LogPropertyName}: {RequestId}",
                LogPropertyNames.XRequestId,
                requestId.ToString()
            );
            //LogContext.PushProperty(LogPropertyNames.XRequestId, requestId);
        }
    }

    /// <summary>
    /// Khởi tạo không có httpContext
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceProvider"></param>
    protected ServiceAbstract(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _localization = serviceProvider.GetRequiredService<LocalizationBase>();
        _httpContext = default!;
    }

    protected ServiceAbstract(
        ILogger logger,
        IHttpContextAccessor httpContext,
        ILocalization localization
    )
    {
        _logger = logger;
        _localization = localization;
        _httpContext = httpContext;

        if (
            _httpContext
                .HttpContext?.Request
                .Headers.TryGetValue(HeaderNames.XRequestId, out var requestId) == true
        )
        {
            _logger.LogInformation(
                "{LogPropertyName}: {RequestId}",
                LogPropertyNames.XRequestId,
                requestId.ToString()
            );
            //LogContext.PushProperty(LogPropertyNames.XRequestId, requestId);
        }
    }

    /// <summary>
    /// Dịch sang ngôn ngữ đích dựa theo keyName và request ngôn ngữ là gì <br/>
    /// Input: <paramref name="keyName"/> = "error_System" <br/>
    /// Return: "Error System" hoặc "Lỗi" tuỳ theo request ngôn ngữ đang là gì ví dụ ở đây là "en" và "VI"
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    protected string L(string keyName)
    {
        return _localization.Localize(keyName);
    }

    /// <summary>
    /// Dịch sang ngôn ngữ đích dựa theo keyName và request ngôn ngữ là gì và dùng <c>string.Format()</c> để format chuỗi<br/>
    /// Ví dụ có thẻ <c>&lt;text name="hello"&gt;Xin chào {0}, {1} tuổi&lt;/text&gt;</c> trong file <c>xml</c> <br/>
    /// Input: <paramref name="keyName"/> = "hello" <paramref name="values"/> = ["Minh", 20] <br/>
    /// Return: "Xin chào Minh, 20 tuổi"
    /// </summary>
    /// <returns></returns>
    protected string L(string keyName, params string[] values)
    {
        return string.Format(_localization.Localize(keyName), values);
    }
}