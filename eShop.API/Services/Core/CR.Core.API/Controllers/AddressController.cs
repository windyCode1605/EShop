using CR.Core.ApplicationServices.AddressModule.Abstracts;
using CR.Core.Dtos.AddressModuleDto;
using CR.DtoBase;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;

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
        => (await _addressService.GetAddressesByUserIdAsync(cancellationToken)).ToActionResult(this);

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

        return (await _addressService.SaveNewAddressAsync(dto, cancellationToken)).ToActionResult(this, "Tao dia chi thanh cong");
    }

    /// <summary>
    /// Đặt một địa chỉ đã tồn tại làm mặc định.
    /// </summary>
    [HttpPatch("{addressId:int}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SetAsDefault(int addressId, CancellationToken cancellationToken)
        => (await _addressService.SetAsDefaultAsync(addressId, cancellationToken)).ToActionResult(this);
}