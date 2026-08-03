// CR.API/Controllers/AdminOrderController.cs
using CR.Core.ApplicationServices.OrderModule.Abstracts;
using CR.Core.ApplicationServices.OrderModule.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetAll([FromQuery] FilterOrderPagingDto input)
        => Ok(await _orderService.GetAllOrders(input));

    /// <summary>Cập nhật trạng thái đơn hàng thủ công (Admin).</summary>
    [HttpPatch("{orderId:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] int orderId,
        [FromBody] UpdateOrderStatusDto input)
        => Ok(await _orderService.UpdateOrderStatus(orderId, input));
}