using CR.Core.API.Extensions;
using CR.Core.ApplicationServices.EmployeeModule.Abstracts;
using CR.Core.Dto.EmployeeDto;
using CR.Utils.Net.Request;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers
{
    [ApiController]
    [Route("api/admin/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployee(EmployeeQueryDto query)
            => (await _employeeService.GetEmployeeAsync(query)).ToActionResult(this);
    }
}