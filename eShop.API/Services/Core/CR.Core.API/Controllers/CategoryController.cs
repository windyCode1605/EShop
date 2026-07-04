using CR.Core.Application.CategoryModule.Abstract;
using CR.Core.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetCategory()
        => Ok(await _categoryService.GetCategories());
}