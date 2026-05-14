using CR.Core.ApplicationServices.OrderModule.Abstracts;
using CR.Core.ApplicationServices.OrderModule.Dtos;
using CR.DtoBase;
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
    
}