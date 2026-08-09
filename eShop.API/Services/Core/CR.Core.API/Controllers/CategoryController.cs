using CR.Core.Application.CategoryModule.Abstract;
using CR.Core.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;
using CR.Core.Dtos.CategoryDto;

namespace CR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpGet("getCategory")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategory()
        => (await _categoryService.GetCategories()).ToActionResult(this);

    [HttpPost("admin/Category")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatCategory(CreateCategoryDto dto)
        => (await _categoryService.CreateCategoryAsync(dto)).ToActionResult(this);
    [HttpPatch("admin/{id}/Category")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(int id)
        => (await _categoryService.UpdateStatusAsync(id)).ToActionResult(this);
}