using CR.Core.ApplicationServices.AddressModule.Abstracts;
using CR.Core.Dtos.AddressModuleDto;
using CR.DtoBase;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[Controller]")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;
    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AddressResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAddress(CancellationToken cancellationToken)
    {
        var result = await _addressService.GetAddressesByUserIdAsync(cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<List<AddressResponseDto>>.Fail($"Lỗi: {result.ErrorCode}"));
        }
        return Ok(ApiResponse<List<AddressResponseDto>>.Ok(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SaveNewAddress(
        [FromBody] SaveAddressRequestDto dto,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Dữ liệu địa chỉ không hợp lệ."));

        var result = await _addressService.SaveNewAddressAsync(dto, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail($"Lỗi: {result.ErrorCode}"));
        }

        return CreatedAtAction(
            nameof(GetAddress),
            null,
            ApiResponse<int>.Ok(result.Value, "Tao dia chi thanh cong")
        );
    }

    /// <summary>
    /// Đặt một địa chỉ đã tồn tại làm mặc định.
    /// </summary>
    [HttpPatch("{addressId:int}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SetAsDefault(int addressId, CancellationToken cancellationToken)
    {
        var result = await _addressService.SetAsDefaultAsync(addressId, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Fail($"Lỗi đặt mặc định: {result.ErrorCode}"));
        }
        return NoContent();
    }
}