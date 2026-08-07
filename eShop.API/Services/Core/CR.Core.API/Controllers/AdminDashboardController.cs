using CR.Core.ApplicationServices.DashboardModule.Abstracts;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
// [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
        => _dashboardService = dashboardService;

    /// <summary>Lấy toàn bộ dữ liệu cho màn hình Admin Dashboard.</summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummary()
        => (await _dashboardService.GetDashboardSummaryAsync()).ToActionResult(this);
}
