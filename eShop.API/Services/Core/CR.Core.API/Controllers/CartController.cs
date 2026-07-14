using CR.Core.ApplicationServices.CartModule.Abstracts;
using CR.Core.ApplicationServices.CartModule.Dtos;
using CR.DtoBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private ICartService _cartService;
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("get-my-cart")]
    public async Task<IActionResult> getCart()
        => Ok(await _cartService.GetCartAsync());


    [HttpPost("add-to-cart")]
    [ProducesResponseType(typeof(Result<AddToCartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto input)
        => Ok(await _cartService.AddItem(input));

    [HttpPut("update-item")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemDto input)
        => Ok(await _cartService.UpdateItem(input));

    [HttpDelete("remove-item")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItem([FromBody] int cartItemId)
        => Ok(await _cartService.RemoveItem(cartItemId));

    [HttpDelete("clear")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCart()
        => Ok(await _cartService.ClearCart());

    /// <summary>
    /// Validate trạng thái giỏ hàng: check stock, biến thể, v.v.
    /// </summary>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(Result<CartValidationResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateCart()
        => Ok(await _cartService.ValidateCart());
    /// <summary>
    /// Tính toán preview đơn hàng: subtotal, phí ship, giảm giá, tổng tiền.
    /// Có thể nhập CouponCode để xem discount trước khi đặt.
    /// </summary>
    [HttpPost("checkout-preview")]
    [ProducesResponseType(typeof(Result<CheckoutPreviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckoutPreview([FromBody] CheckoutPreviewRequestDto input)
        => Ok(await _cartService.ChechoutPerview(input));
}