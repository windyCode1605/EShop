using CR.ApplicationBase.Common;
using CR.Constants.Core.Users;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.CustomerModule.Implements
{
    public class AdminCustomerService : CoreServiceBase, IAdminCustomerService
    {
        public AdminCustomerService(ILogger<AdminCustomerService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public async Task<Result<PageResult<CustomerListItemDto>>> GetAllCusAsync(CustomerAdminQueryDto query)
        {
            _logger.LogInformation("Method : {method}", nameof(GetAllCusAsync));
            var baseQuery = _dbContext.Users
                .AsNoTracking()
                .Where(c => c.UserType == UserTypeEnum.CUSTOMER && !c.Deleted);
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var kw = query.Keyword.Trim().ToLower();
                baseQuery = baseQuery.Where(c =>
                    c.Username.ToLower().Contains(kw) ||
                    c.Email.ToLower().Contains(kw) ||
                    (c.Phone != null && c.Phone.Contains(kw)) ||
                    (c.Profile != null && c.Profile.FullName != null && c.Profile.FullName.ToLower().Contains(kw))
                );
            }
            if (query.IsActive.HasValue)
            {
                if (query.IsActive.Value)
                {
                    baseQuery = baseQuery.Where(c => c.Status == UserStatus.ACTIVE);
                }
                else
                {
                    // Trả về tất cả trạng thái KHÔNG phải ACTIVE (như DEACTIVE, LOCK, TEMP...)
                    baseQuery = baseQuery.Where(c => c.Status != UserStatus.ACTIVE);
                }
            }
            if (query.FromDate.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.CreatedDate >= query.FromDate.Value);
            }
            if (query.ToDate.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.CreatedDate <= query.ToDate.Value);
            }

            var totalItems = await baseQuery.CountAsync();
            var sortedQuery = query.Sort?.Any() == true
                ? baseQuery.OrderDynamic(query.Sort)
                : baseQuery.OrderByDescending(c => c.CreatedDate);
            var items = await sortedQuery
                .Paging(query)
                .Select(c => new CustomerListItemDto
                {
                    Id = c.Id,
                    Username = c.Username,
                    Email = c.Email,
                    FullName = c.Profile != null && c.Profile.FullName != null ? c.Profile.FullName : string.Empty,
                    Phone = c.Phone ?? string.Empty,
                    IsActive = c.Status == UserStatus.ACTIVE,
                    CreatedAt = c.CreatedDate,
                    LastLoginDate = c.LastLogin,
                    TotalOrders = c.Orders.Count(o => !o.Deleted),
                    TotalSpent = c.Orders.Where(o => !o.Deleted).Sum(o => (decimal?)o.TotalAmount) ?? 0
                })
                .ToListAsync();
            var pageResult = PageResult<CustomerListItemDto>.Create(items, totalItems, query);
            return Result<PageResult<CustomerListItemDto>>.Success(pageResult);
        }
    }
}