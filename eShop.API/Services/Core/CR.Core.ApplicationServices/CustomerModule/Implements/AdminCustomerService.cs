using CR.ApplicationBase.Common;
using CR.Constants.Core.Users;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;
using CR.Utils.DataUtils;
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
        public async Task<Result<CustomerDetail360Dto>> GetCustomerDetailAsync(int id)
        {
            _logger.LogInformation("Method : {method} Customer id = {ID}", nameof(GetCustomerDetailAsync), id);

            var customer = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserType == UserTypeEnum.CUSTOMER && u.Id == id)
                .Select(c => new CustomerDetail360Dto
                {
                    Id = c.Id,
                    Username = c.Username,
                    Email = c.Email,
                    FullName = c.Profile.FullName ?? string.Empty,
                    Phone = c.Phone ?? string.Empty,
                    IsActive = c.Status == UserStatus.ACTIVE,
                    CreatedAt = c.CreatedDate,
                    LastLoginDate = c.LastLogin,
                    Addresses = c.Addresses.Select(a => new CustomerAddressItemDto
                    {
                        Id = a.Id,
                        ReceiverName = a.ReceiverName,
                        Phone = a.ReceiverPhone,
                        FullAddress = a.City + " " + a.Province + " " + a.Street,
                        IsDefault = a.IsDefault,
                    }).ToList()
                })
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return Result<CustomerDetail360Dto>.Failure(
                    CR.Constants.ErrorCodes.ErrorCode.UserNotFound,
                    "Không tìm thấy thông tin khách hàng"
                );
            }

            return Result<CustomerDetail360Dto>.Success(customer);
        }
        // Lấy tổng số tiền và tổng số đơn hàng của 1 Customer
        public async Task<Result<CustomerStatisticsDto>> GetStatisticsAsync(int id)
        {
            _logger.LogInformation("Method : {method} , ID : {id}", nameof(GetStatisticsAsync), id);

            var stats = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id && u.UserType == UserTypeEnum.CUSTOMER)
                .Select(u => new CustomerStatisticsDto
                {
                    TotalOrders = u.Orders.Count(o => !o.Deleted),
                    TotalSpent = u.Orders.Where(o => !o.Deleted).Sum(o => (decimal?)o.TotalAmount) ?? 0,
                    RecentOrders = u.Orders
                        .Where(o => !o.Deleted)
                        .OrderByDescending(o => o.CreatedDate)
                        .Take(5)
                        .Select(o => new CustomerRecentOrderItemDto
                        {
                            OrderId = o.Id,
                            OrderCode = o.OrderCode,
                            OrderStatus = o.Status,
                            OrderDate = o.CreatedDate,
                            TotalAmount = o.TotalAmount,
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (stats == null)
            {
                return Result<CustomerStatisticsDto>.Failure(
                    CR.Constants.ErrorCodes.ErrorCode.UserNotFound,
                    "Không tìm thấy thông tin khách hàng"
                );
            }

            return Result<CustomerStatisticsDto>.Success(stats);
        }

        public async Task<Result<bool>> LockCustomerAccount(int id)
        {
            _logger.LogInformation("Method: {method}, Customer ID : {id}", nameof(LockCustomerAccount), id);
            var affectedRows = await _dbContext.Users
            .Where(u =>
                u.Id == id &&
                !u.Deleted &&
                u.UserType == UserTypeEnum.CUSTOMER &&
                (
                    u.Status == UserStatus.ACTIVE ||
                    u.Status == UserStatus.TEMP ||
                    u.Status == UserStatus.TEMP_OTP
                ))
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.Status, UserStatus.LOCK));

            if (affectedRows == 0)
                return Result<bool>.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());

            return Result<bool>.Success(true);
        }
        public async Task<Result<bool>> UnlockCustomerAccount(int id)
        {
            _logger.LogInformation("Method: {method}, Customer ID : {id}", nameof(UnlockCustomerAccount), id);
            var affectedRows = await _dbContext.Users
            .Where(u =>
                u.Id == id &&
                !u.Deleted &&
                u.UserType == UserTypeEnum.CUSTOMER &&
                (u.Status == UserStatus.LOCK || u.Status == UserStatus.DEACTIVE))
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.Status, UserStatus.ACTIVE));

            if (affectedRows == 0)
                return Result<bool>.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());

            return Result<bool>.Success(true);
        }
    }
}