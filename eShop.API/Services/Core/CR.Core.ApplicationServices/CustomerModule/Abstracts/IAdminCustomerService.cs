using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.CustomerModule.Abstracts
{
    public interface IAdminCustomerService
    {
        Task<Result<PageResult<CustomerListItemDto>>> GetAllCusAsync(CustomerAdminQueryDto query);
        Task<Result<CustomerDetail360Dto>> GetCustomerDetailAsync(int id);
        Task<Result<CustomerStatisticsDto>> GetStatisticsAsync(int id);
        Task<Result<bool>> LockCustomerAccount(int customerId);
        Task<Result<bool>> UnlockCustomerAccount(int customerId);
        // Task<Result<bool>> AdjustCustomerPoints(int customerId, int points, string reason);
    }
}