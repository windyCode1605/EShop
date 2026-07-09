using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.WebAPIBase.Responses;


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
        public async Task<IActionResult> AssignRole([FromRoute] int userId, [FromBody] AssignRoleDto input)
        {
            var result = await _userService.AssignRoleToUser(userId, input.RoleId);
            if (result.IsFailure)
                return BadRequest(ApiResponse<bool>.Fail("Cấp quyền thất bại", result.ErrorCode));

            return Ok(ApiResponse<bool>.Ok(true, "Cấp quyền thành công"));
        }
    }
}