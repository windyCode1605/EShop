using CR.ApplicationBase.Common;
using CR.Constants.Core.Users;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.EmployeeModule.Abstracts;
using CR.Core.Dto.EmployeeDto;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.EmployeeModule.Iplement
{
    public class EmployeeService : CoreServiceBase, IEmployeeService
    {
        public EmployeeService(
            ILogger<EmployeeService> logger,
            IHttpContextAccessor httpContext
            ) : base(logger, httpContext) { }

        public async Task<Result<PageResult<EmployeeResponseDto>>> GetEmployeeAsync(EmployeeQueryDto query)
        {
            _logger.LogInformation("Method {method}", nameof(GetEmployeeAsync));
            var baseQuery = _dbContext.Users
                .Where(u => !u.Deleted && (u.UserType == UserTypeEnum.ADMIN || u.UserType == UserTypeEnum.SUPER_ADMIN));
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var kw = query.Keyword.Trim().ToLower();
                baseQuery = baseQuery.Where(e =>
                        e.Username.ToLower().Contains(kw) ||
                        e.Email.ToLower().Contains(kw) ||
                        (e.Phone != null && e.Phone.ToLower().Contains(kw)) ||
                        (e.Profile != null && e.Profile.FullName != null && e.Profile.FullName.ToLower().Contains(kw)));
            }
            if (query.IsActive.HasValue)
            {
                baseQuery = baseQuery.Where(e => e.Status == UserStatus.ACTIVE);
            }
            if (query.FormDate.HasValue)
                baseQuery = baseQuery.Where(e => e.CreatedDate >= query.FormDate);
            if (query.ToDate.HasValue)
                baseQuery = baseQuery.Where(e => e.CreatedDate <= query.ToDate);
            var totalE = await baseQuery.CountAsync();
            var sortedQuery = query.Sort?.Any() == true
                ? baseQuery.OrderDynamic(query.Sort)
                : baseQuery.OrderByDescending(e => e.CreatedDate);
            var items = await sortedQuery
                .Paging(query)
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Username = e.Username,
                    Email = e.Email,
                    PhoneNumber = e.Phone,
                    CreatDate = e.CreatedDate,
                }).ToArrayAsync();
            var Result = PageResult<EmployeeResponseDto>.Create(items, totalE, query);
            return Result<PageResult<EmployeeResponseDto>>.Success(Result);
        }
    }
}