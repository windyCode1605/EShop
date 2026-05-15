// using CR.DtoBase;

// namespace CR.MongoDb;

// public class Repository<TEntity, TFilter, TUpdate> : IRepository<TEntity, TFilter>
// {
//     protected readonly ILogger _logger;
//     protected readonly int? UserId;
//     public Repository(
//             ILogger logger,
//             IHttpContextAccessor httpContextAccessor,
//             IMongoDatabase database,
//             string collectionName
//         )
//         {
//             if (
//                 httpContextAccessor
//                     .HttpContext?.Request
//                     .Headers.TryGetValue(HeaderNames.XRequestId, out var requestId) == true
//             )
//             {
//                 XRequestId = requestId;
//                 Serilog
//                     .Log.ForContext<ILogger>()
//                     .ForContext(LogPropertyNames.XRequestId, requestId);
//             }
//             _logger = logger;
//             _collection = database.GetCollection<TEntity>(collectionName);
//             UserId = httpContextAccessor.GetCurrentUserIdInContext();
//             TenantId = httpContextAccessor.GetCurrentTenantIdInContext();
//         }
//     public async Task<PagingResult<TEntity>> GetPaginatedAsync(int pageNumber, int pageSize)
//         {
//             _logger.LogInformation(
//                 "{Repo}->{Method}: {PageNumber} = {PageNumberData}, {PageSize} = {PageSizeData}",
//                 GetType().FullName,
//                 nameof(GetPaginatedAsync),
//                 nameof(pageNumber),
//                 pageNumber,
//                 nameof(pageSize),
//                 pageSize
//             );
//             return await GetPaginatedAsync(CreateFilter(), pageNumber, pageSize);
//         }

//         protected async Task<PagingResult<TEntity>> GetPaginatedAsync(
//             FilterDefinition<TEntity> filter,
//             int pageNumber,
//             int pageSize
//         )
//         {
//             filter = filter & CreateFilter();
//             _logger.LogInformation(
//                 "{Repo}->{Method}: {Filter} = {FilterData}, {PageNumber} = {PageNumberValue}, {PageSize} = {PageSizeValue}",
//                 GetType().FullName,
//                 nameof(GetPaginatedAsync),
//                 nameof(filter),
//                 filter.ToJson(),
//                 nameof(pageNumber),
//                 pageNumber,
//                 nameof(pageSize),
//                 pageSize
//             );
//             var totalItems = await _collection.CountDocumentsAsync(filter);
//             var items = await _collection
//                 .Find(filter)
//                 .Skip((pageNumber - 1) * pageSize)
//                 .Limit(pageSize)
//                 .ToListAsync();

//             return new PagingResult<TEntity> { Items = items, TotalItems = totalItems };
//         }

//         /// <summary>
//         /// Lấy phân trang theo limit offset, filter mặc định theo tenantId nếu tenantId khác null
//         /// </summary>
//         /// <param name="filter"></param>
//         /// <param name="offset"></param>
//         /// <param name="limit"></param>
//         /// <returns></returns>
//         protected async Task<PageResult<TEntity>> GetPaginatedLimitOffsetAsync(
//             FilterDefinition<TEntity> filter,
//             int offset,
//             int limit
//         )
//         {
//             filter = filter & CreateFilter();
//             _logger.LogInformation(
//                 "{Repo}->{Method}: {Filter} = {FilterData}, {Offset} = {OffsetValue}, {Limit} = {LimitValue}",
//                 GetType().FullName,
//                 nameof(GetPaginatedLimitOffsetAsync),
//                 nameof(filter),
//                 filter.ToJson(),
//                 nameof(offset),
//                 offset,
//                 nameof(limit),
//                 limit
//             );
//             var totalItems = await _collection.CountDocumentsAsync(filter);
//             var items = await _collection.Find(filter).Skip(offset).Limit(limit).ToListAsync();

//             return new PageResult<TEntity> { Items = items, TotalItems = totalItems };
//         }
// }
