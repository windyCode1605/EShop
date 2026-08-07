using CR.Core.Dtos.DashboardModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.DashboardModule.Abstracts;

public interface IDashboardService
{
    Task<Result<DashboardResponseDto>> GetDashboardSummaryAsync();
}
