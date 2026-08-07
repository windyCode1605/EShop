using CR.Core.ApplicationServices.AttributeModule.Abstract;
using CR.Core.Dtos.AttributeModule;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;

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
            => (await _attributeService.CreateAsync(input)).ToActionResult(this);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => (await _attributeService.GetByIdAsync(id)).ToActionResult(this);

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _attributeService.GetAllAsync(page, size);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttribute(int id, [FromBody] AttributeRequest input)
            => (await _attributeService.UpdateAsync(id, input)).ToActionResult(this);

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute(int id)
            => (await _attributeService.DeleteAsync(id)).ToActionResult(this);

        [HttpGet("value")]
        public async Task<IActionResult> GetValuesByAttributeIdAsync([FromQuery] FilterAttributeValuePagingDto input)
            => (await _attributeValueService.GetValuesByAttributeIdAsync(input)).ToActionResult(this);
        [HttpPost("value")]
        public async Task<IActionResult> CreatedAttributeValue(AttributeValueRequestDto input)
            => (await _attributeValueService.CreateAsync(input)).ToActionResult(this);
    }
}