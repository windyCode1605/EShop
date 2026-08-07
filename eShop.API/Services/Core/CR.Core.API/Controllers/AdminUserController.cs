using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.WebAPIBase.Responses;
using CR.Core.API.Extensions;


namespace CR.Core.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "ADMIN")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("{userId:int}/roles")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole([FromRoute] int userId, [FromBody] AssignRoleDto input)
            => (await _userService.AssignRoleToUser(userId, input.RoleId)).ToActionResult(this, "Cấp quyền thành công");

        [HttpGet("test-permission")]
        [Authorize(Policy = "Permission:AdminUser_Read")]
        public IActionResult TestPermission()
        {
            return Ok(ApiResponse<string>.Ok("Bạn đã qua được chốt kiểm tra quyền!", "Success"));
        }
    }
}