// CR.API/Controllers/ShipmentController.cs
using CR.Core.ApplicationServices.ShipmentModule.Abstracts;
using CR.Core.Dtos.Shipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;

[ApiController]
[Route("api/shipments")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    public ShipmentController(IShipmentService shipmentService)
        => _shipmentService = shipmentService;

    /// <summary>Lấy thông tin vận chuyển của đơn hàng.</summary>
    [HttpGet("order/{orderId:int}")]
    // [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
        => (await _shipmentService.GetOrderById(orderId)).ToActionResult(this);

    /// <summary>
    /// Webhook từ GHN/NinjaVan — KHÔNG cần Authorize.
    /// Carrier gọi về khi trạng thái vận đơn thay đổi.
    /// </summary>
    [HttpPost("webhook")]
    // [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShipmentWebhook([FromBody] ShipmentWebhookDto input)
        => (await _shipmentService.HandleShipmentWebhook(input)).ToActionResult(this);

    /// <summary>Admin cập nhật mã tracking sau khi bàn giao shipper.</summary>
    [HttpPatch("tracking")]
    // [Authorize(Roles = "ADMIN,STAFF")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTracking([FromBody] UpdateTrackingDto input)
        => (await _shipmentService.UpdateTracking(input)).ToActionResult(this);

    /// <summary>Admin cập nhật trạng thái vận đơn thủ công.</summary>
    [HttpPatch("{shipmentId:int}/status")]
    // [Authorize(Roles = "ADMIN,STAFF")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] int    shipmentId,
        [FromQuery] string newStatus)
        => (await _shipmentService.UpdateShipmentStatus(shipmentId, newStatus)).ToActionResult(this);
}