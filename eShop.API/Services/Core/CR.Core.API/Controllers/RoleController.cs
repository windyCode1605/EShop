using System.Reflection.Metadata.Ecma335;
using CR.Core.ApplicationServices.RoleModule.Abstracts;
using CR.Core.Dtos.RoleModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;

namespace CR.Core.API.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CR.WebAPIBase.Responses.ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole(CreateRoleRequest dto)
            => (await _roleService.CreateRoleAsync(dto)).ToActionResult(this, "Thành công");
    }
}