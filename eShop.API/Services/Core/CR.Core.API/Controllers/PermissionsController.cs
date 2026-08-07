using CR.Core.ApplicationServices.RoleModule;
using CR.Core.Dtos.RoleModule;
using CR.Utils.Net.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;

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
            => (await _permissionService.GetAllPermissionAsync()).ToActionResult(this, "Thành công");
    }
}