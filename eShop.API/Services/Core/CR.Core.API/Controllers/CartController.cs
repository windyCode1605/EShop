using CR.Core.ApplicationServices.CartModule.Abstracts;
using CR.Core.ApplicationServices.CartModule.Dtos;
using CR.DtoBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;

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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> getCart()
        => (await _cartService.GetCartAsync()).ToActionResult(this);


    [HttpPost("add-to-cart")]
    [ProducesResponseType(typeof(ApiResponse<AddToCartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto input)
        => (await _cartService.AddItem(input)).ToActionResult(this);

    [HttpPut("update-item")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemDto input)
        => (await _cartService.UpdateItem(input)).ToActionResult(this);

    [HttpDelete("remove-item")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem([FromBody] int cartItemId)
        => (await _cartService.RemoveItem(cartItemId)).ToActionResult(this);

    [HttpDelete("clear")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClearCart()
        => (await _cartService.ClearCart()).ToActionResult(this);

    /// <summary>
    /// Validate trạng thái giỏ hàng: check stock, biến thể, v.v.
    /// </summary>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(ApiResponse<CartValidationResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateCart()
        => (await _cartService.ValidateCart()).ToActionResult(this);
    /// <summary>
    /// Tính toán preview đơn hàng: subtotal, phí ship, giảm giá, tổng tiền.
    /// Có thể nhập CouponCode để xem discount trước khi đặt.
    /// </summary>
    [HttpPost("checkout-preview")]
    [ProducesResponseType(typeof(ApiResponse<CheckoutPreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckoutPreview([FromBody] CheckoutPreviewRequestDto input)
        => (await _cartService.ChechoutPerview(input)).ToActionResult(this);
}