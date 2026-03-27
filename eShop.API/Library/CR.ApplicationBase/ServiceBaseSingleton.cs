// Library/CR.ApplicationBase/ServiceBaseSingleton.cs
using Microsoft.Extensions.Logging;

namespace CR.ApplicationBase;

// Dùng cho service không cần DbContext (vd: cache, background job)
public abstract class ServiceBaseSingleton
{
    protected readonly ILogger _logger;

    protected ServiceBaseSingleton(ILogger logger)
    {
        _logger = logger;
    }
}