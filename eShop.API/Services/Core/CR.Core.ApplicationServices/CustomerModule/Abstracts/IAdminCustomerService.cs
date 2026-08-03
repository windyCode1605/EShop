using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.CustomerModule.Abstracts
{
    public interface IAdminCustomerService
    {
        Task<Result<PageResult<CustomerListItemDto>>> GetAllCusAsync(CustomerAdminQueryDto query);
        // Task<Result<CustomerDetail360Dto>> CustomerDetail360(int id);
        // Task<Result<bool>> LockCustomerAccount(int customerId, string reason);
        // Task<Result<bool>> AdjustCustomerPoints(int customerId, int points, string reason);
    }
}