using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.Dtos.CustomerModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.DtoBase;
using CR.Utils.Net.Request;
using Google.Api.Gax.Rest;
using CR.Core.Domain.User;


namespace CR.Core.API.Controllers;


[ApiController]
[Route("api/admin/customers")]
public class AdminCustomerController : ControllerBase
{
    private readonly IAdminCustomerService _adminCustomerService;
    public AdminCustomerController(IAdminCustomerService adminCustomerService)
    {
        _adminCustomerService = adminCustomerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PageResult<CustomerListItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(CustomerAdminQueryDto query) // 2. Tường minh Binding
    {
        var result = await _adminCustomerService.GetAllCusAsync(query);
        return result.ToActionResult(this);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetail360Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _adminCustomerService.GetCustomerDetailAsync(id);
        return result.ToActionResult(this);
    }

    [HttpGet("{id}/statistics")]
    [ProducesResponseType(typeof(ApiResponse<CustomerStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatistics(int id)
    {
        var result = await _adminCustomerService.GetStatisticsAsync(id);
        return result.ToActionResult(this);
    }
    [HttpPost("{id}/lock")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LockCustomerAccount(int id)
    {
        var result = await _adminCustomerService.LockCustomerAccount(id);
        return result.ToActionResult(this);
    }
    [HttpPost("{id}/unlock")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlockCustomerAccount(int id)
    {
        var result = await _adminCustomerService.UnlockCustomerAccount(id);
        return result.ToActionResult(this);
    }
}