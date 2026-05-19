using CR.Core.ApplicationServices.OrderModule.Abstracts;
using CR.Core.ApplicationServices.OrderModule.Dtos;
using CR.DtoBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    /// <summary>
    /// Tạo đơn hàng mới từ giỏ hàng hiện tại của người dùng
    /// - Yêu cầu: Người dùng phải đã đăng nhập và có giỏ hàng chứa sản phẩm
    /// - Input: Thông tin địa chỉ giao hàng (có thể chọn từ địa
    /// chỉ đã lưu hoặc nhập mới), phương thức thanh toán, mã giảm giá (nếu có), v.v.
    /// - Output: Thông tin đơn hàng vừa tạo, bao gồm mã đơn hàng,
    /// danh sách sản phẩm, tổng tiền, trạng thái đơn hàng, v.v.
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto input)
        => Ok(await _orderService.CreateOrder(input));

    /// <summary>
    /// Lấy thông tin chi tiết của một đơn hàng cụ thể
    /// </summary>
    [Authorize]
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetById([FromRoute]int orderId)
        => Ok(await _orderService.GetById(orderId));

    /// <summary>
    /// Lấy danh sách đơn hàng của người dùng hiện tại với phân trang
    /// </summary>
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] FilterOrderPagingDto input)
        => Ok(await _orderService.GetMyOrders(input));
    
    /// <summary>
    /// Hủy đơn hàng ( chỉ khi PENDING hoặc CONFIRMED)
    /// </summary>
    [HttpPost("{orderId:int}/cancel")]
    public async Task<IActionResult> CancelOrder([FromRoute] int orderId, [FromQuery] string? reason = null)
        => Ok(await _orderService.CancelOrder(orderId, reason));
}