// CR.API/Controllers/AdminOrderController.cs
using CR.Core.ApplicationServices.OrderModule.Abstracts;
using CR.Core.ApplicationServices.OrderModule.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;

namespace CR.Core.API.Controllers;
[ApiController]
[Route("api/admin/orders")]
// [Authorize(Roles = "ADMIN,STAFF")]
public class AdminOrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public AdminOrderController(IOrderService orderService) => _orderService = orderService;

    /// <summary>Danh sách tất cả đơn hàng (Admin).</summary>
    [HttpGet]
    [Authorize(Policy = "Permission:Sales.Orders.View")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] FilterOrderPagingDto input)
        => (await _orderService.GetAllOrders(input)).ToActionResult(this);

    /// <summary>Cập nhật trạng thái đơn hàng thủ công (Admin).</summary>
    [HttpPatch("{orderId:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] int orderId,
        [FromBody] UpdateOrderStatusDto input)
        => (await _orderService.UpdateOrderStatus(orderId, input)).ToActionResult(this);
}