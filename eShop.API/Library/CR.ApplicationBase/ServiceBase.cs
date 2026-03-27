// Base class cho tất cả service trong solution
// Cung cấp logger và DbContext sẵn - không phải inject riêng từng service
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CR.ApplicationBase.Localization;
using CR.EntitiesBase;
using AutoMapper;
using CR.Core.ApplicationServices.Common.ServiceImplementations;

namespace CR.ApplicationBase;

public abstract class ServiceBase<TContext> where TContext : DbContext
{
    protected readonly TContext _db;
    protected readonly ILogger _logger;
    protected readonly IMapper _mapper;
    // private CoreDbContext db;
    // private IMapper mapper;

    protected ServiceBase(TContext db, ILogger logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }

    // protected ServiceBase(CoreDbContext db, ILogger<ProductService> logger, IMapper mapper)
    // {
    //     this.db = db;
    //     _logger = logger;
    //     this.mapper = mapper;
    // }

    // Helper: lấy tất cả, không lấy soft-deleted
    protected IQueryable<T> QueryActive<T>() where T : BaseEntity
    => _db.Set<T>().Where(x => !x.IsDeleted);
}
