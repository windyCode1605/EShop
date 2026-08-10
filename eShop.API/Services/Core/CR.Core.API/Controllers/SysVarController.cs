using CR.Core.ApplicationServices.SysvarModule.Abstracts;
using CR.Core.API.Extensions;
using CR.Core.Dto.SysvarModule;
using CR.DtoBase;
using CR.Utils.Net.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;

namespace CR.Core.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class SysVarController : ControllerBase
    {
        private readonly ISysvarService _sysService;
        public SysVarController(ISysvarService sysvarService)
        {
            _sysService = sysvarService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<SysvarResponsDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllSysVar()
            => (await _sysService.GetSysvarsAsync()).ToActionResult(this);
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<List<SysvarUpdateDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSysVar(int id, SysvarUpdateDto dto)
            => (await _sysService.UpdateSysVarAsync(id, dto)).ToActionResult(this);

    }
}