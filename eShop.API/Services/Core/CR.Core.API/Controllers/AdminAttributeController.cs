using CR.Core.ApplicationServices.AttributeModule.Abstract;
using CR.Core.Dtos.AttributeModule;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers
{

    [ApiController]
    [Route("api/attribute/[controller]")]
    public class AdminAttributeController : ControllerBase
    {
        private readonly IAttributeService _attributeService;
        private readonly IAttributeValueService _attributeValueService;
        public AdminAttributeController(IAttributeService attributeService, IAttributeValueService attributeValueService)
        {
            _attributeService = attributeService;
            _attributeValueService = attributeValueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttribute([FromBody] AttributeRequest input)
        {
            var result = await _attributeService.CreateAsync(input);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _attributeService.GetByIdAsync(id);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _attributeService.GetAllAsync(page, size);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttribute(int id, [FromBody] AttributeRequest input)
        {
            var result = await _attributeService.UpdateAsync(id, input);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute(int id)
        {
            var result = await _attributeService.DeleteAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("value")]
        public async Task<IActionResult> GetValuesByAttributeIdAsync([FromQuery] FilterAttributeValuePagingDto input)
        {
            var result = await _attributeValueService.GetValuesByAttributeIdAsync(input);
            if (result.IsFailure)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("value")]
        public async Task<IActionResult> CreatedAttributeValue(AttributeValueRequestDto input)
        {
            var result = await _attributeValueService.CreateAsync(input);
            if (result.IsFailure)
                return BadRequest(result);
            return Ok(result);
        }
    }
}