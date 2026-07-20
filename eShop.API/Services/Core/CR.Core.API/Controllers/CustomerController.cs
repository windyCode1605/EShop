using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.Dtos.CustomerModule;
using CR.Core.Domain.User;
using CR.DtoBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;

namespace CR.Core.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMyProfile()
            => (await _customerService.GetMyProfile()).ToActionResult(this, "Lấy thông tin hồ sơ thành công.");


        [HttpPatch("me")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto input)
            => (await _customerService.UpdateMyProfile(input)).ToActionResult(this, "Cập nhật hồ sơ thành công.");
    }
}