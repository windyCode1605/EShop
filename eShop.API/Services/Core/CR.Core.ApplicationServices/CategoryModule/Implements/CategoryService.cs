using CR.Core.Application.CategoryModule.Abstract;
using CR.Core.ApplicationServices.Common;
using CR.Core.Dtos.CategoryDto;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Application.CategoryModule.Implements;

public class CategoryService : CoreServiceBase, ICategoryService
{
    public CategoryService(ILogger<CategoryService> logger, IHttpContextAccessor httpContext)
    : base(logger, httpContext) { }

    public async Task<Result<List<CategoryResponseDto>>> GetCategories()
    {
        _logger.LogInformation("{method} called", nameof(GetCategories));
        var categories = await _dbContext.Categories
            .Select(c => new CategoryResponseDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name
            }).ToListAsync();
        if (!categories.Any())
        {
            return Result<List<CategoryResponseDto>>.Success(new List<CategoryResponseDto>());
        }
        return Result<List<CategoryResponseDto>>.Success(categories);
    }
}