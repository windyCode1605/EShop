// Base class cho tất cả service trong solution
// Cung cấp logger và DbContext sẵn - không phải inject riêng từng service
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CR.ApplicationBase.Localization;
using CR.EntitiesBase;
using AutoMapper;
using CR.Core.ApplicationServices.Common.ServiceImplementations;

namespace CR.ApplicationBase;

public abstract class ServiceBase<TDbContext> : ServiceAbstract where TDbContext : DbContext
{
    protected readonly TDbContext _dbContext;
    protected ServiceBase(ILogger logger, IHttpContextAccessor httpContext)
        : base(logger, httpContext)
    {
        _dbContext = httpContext.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
    }
    // Constructor: nhận logger, dbContext và localization từ DI container
    protected ServiceBase( ILogger logger, IHttpContextAccessor httpContext, ILocalization localization)
    : base(logger, httpContext, localization)
    {
        _dbContext = httpContext.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
    }

    // protected ServiceBase(CoreDbContext db, ILogger<ProductService> logger, IMapper mapper)
    // {
    //     this.db = db;
    //     _logger = logger;
    //     this.mapper = mapper;
    // }

    // Helper: lấy tất cả, không lấy soft-deleted
    protected IQueryable<T> QueryActive<T>() where T : BaseEntity
    => _dbContext.Set<T>().Where(x => !x.IsDeleted);
}
