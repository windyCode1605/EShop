using CR.Core.ApplicationServices.RoleModule;
using CR.Core.Dtos.RoleModule;
using CR.Utils.Net.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers
{
    [Authorize(Roles = "SUPER_ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<List<PermissionGroupDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllPermission()
        {
            var result = await _permissionService.GetAllPermissionAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(CR.WebAPIBase.Responses.ApiResponse<object>.Fail("Lấy danh sách quyền thất bại", result.ErrorCode));
            }
            return Ok(CR.WebAPIBase.Responses.ApiResponse<List<PermissionGroupDto>>.Ok(result.Value, "Thành công"));
        }
    }
}