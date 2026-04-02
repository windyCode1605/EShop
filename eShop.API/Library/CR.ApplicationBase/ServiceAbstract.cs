using CR.ApplicationBase.Localization;
using CR.ConstantBase.Common;
using AppHeaderNames = CR.ConstantBase.Common.HeaderNames;

namespace CR.ApplicationBase;
public abstract class ServiceAbstract
{
    protected readonly ILogger _logger;
    protected readonly ILocalization _localization; // Localization để lấy message theo error code
    protected readonly IHttpContextAccessor _httpContext; // Để lấy DbContext từ DI container trong constructor
    
    protected ServiceAbstract(ILogger logger, IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _httpContext = httpContext;
        _localization = httpContext.HttpContext!.RequestServices.GetRequiredService<LocalizationBase>()
        ?? default!;
        if ( _httpContext.HttpContext!.Request.Headers.TryGetValue(
            AppHeaderNames.XRequestId, out var requestId) == true) 
        {
            _logger.LogInformation("{LogPropertyName}: {RequestId}", LogPropertyNames.XRequestId, requestId.ToString());
        }
    }

    protected ServiceAbstract(ILogger logger, IHttpContextAccessor httpContext, ILocalization localization)
    {
        _logger = logger;
        _httpContext = httpContext;
        _localization = localization;

        if (_httpContext.HttpContext!.Request.Headers.TryGetValue(
            AppHeaderNames.XRequestId, out var requestId) == true)
        {
            _logger.LogInformation("{LogPropertyName}: {RequestId}", LogPropertyNames.XRequestId, requestId.ToString());
        }
    }
}