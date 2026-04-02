using System.Security.Cryptography;
using CR.ApplicationBase;
using CR.ApplicationBase.Localization;

namespace CR.Core.ApplicationServices.Common
{
    public abstract class CoreServiceBase : ServiceBase<CoreDbContext>
    {
        protected CoreServiceBase(ILogger logger, IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
        { }
        protected CoreServiceBase(ILogger logger, IHttpContextAccessor httpContextAccessor, ILocalization localization)
        : base(logger, httpContextAccessor, localization)
        { }
    }
}