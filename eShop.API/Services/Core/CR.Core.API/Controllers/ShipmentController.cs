// CR.API/Controllers/ShipmentController.cs
using CR.Core.ApplicationServices.ShipmentModule.Abstracts;
using CR.Core.Dtos.Shipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
        => Ok(await _shipmentService.GetOrderById(orderId));

    /// <summary>
    /// Webhook từ GHN/NinjaVan — KHÔNG cần Authorize.
    /// Carrier gọi về khi trạng thái vận đơn thay đổi.
    /// </summary>
    [HttpPost("webhook")]
    // [AllowAnonymous]
    public async Task<IActionResult> ShipmentWebhook([FromBody] ShipmentWebhookDto input)
        => Ok(await _shipmentService.HandleShipmentWebhook(input));

    /// <summary>Admin cập nhật mã tracking sau khi bàn giao shipper.</summary>
    [HttpPatch("tracking")]
    // [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> UpdateTracking([FromBody] UpdateTrackingDto input)
        => Ok(await _shipmentService.UpdateTracking(input));

    /// <summary>Admin cập nhật trạng thái vận đơn thủ công.</summary>
    [HttpPatch("{shipmentId:int}/status")]
    // [Authorize(Roles = "ADMIN,STAFF")]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] int    shipmentId,
        [FromQuery] string newStatus)
        => Ok(await _shipmentService.UpdateShipmentStatus(shipmentId, newStatus));
}