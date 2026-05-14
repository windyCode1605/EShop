using CR.ConstantBase.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;


namespace CR.IdentityServerBase.Controllers
{
    public abstract class AuthorizationControllerBase : Controller
    {
        protected readonly ILogger _logger;
        protected readonly IOpenIddictApplicationManager _applicationManager;

        protected AuthorizationControllerBase(
            ILogger logger,
            IOpenIddictApplicationManager applicationManager
        )
        {
            _logger = logger;
            _applicationManager = applicationManager;
            if (HttpContext?.Request?.Headers?.TryGetValue(HeaderNames.XRequestId, out var requestId) == true)
            {
                Serilog
                    .Log.ForContext<ILogger>()
                    .ForContext(LogPropertyNames.XRequestId, requestId);
            }
        }
    }
}
