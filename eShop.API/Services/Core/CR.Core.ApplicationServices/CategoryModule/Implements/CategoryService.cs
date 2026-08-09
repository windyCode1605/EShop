using System.Transactions;
using CR.Constants.ErrorCodes;
using CR.Core.Application.CategoryModule.Abstract;
using CR.Core.ApplicationServices.Common;
using CR.Core.Dtos.CategoryDto;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CR.Core.Application.CategoryModule.Implements;

public class CategoryService : CoreServiceBase, ICategoryService
{
    public CategoryService(ILogger<CategoryService> logger, IHttpContextAccessor httpContext)
    : base(logger, httpContext) { }

    public async Task<Result<List<CategoryResponseDto>>> GetCategories()
    {
        _logger.LogInformation("{method} called", nameof(GetCategories));
        var categories = await _dbContext.Categories
            .Where(c => !c.Deleted)
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
    public async Task<Result> CreateCategoryAsync(CreateCategoryDto dto)
    {
        _logger.LogInformation("Method {Method} called, Data: {@Data}", nameof(CreateCategoryAsync), dto);

        var userId = _httpContext.GetCurrentUserId();

        var category = new Domain.Catalog.Category
        {
            ParentId = dto.ParentId,
            Name = dto.Name,
            Slug = dto.Slug,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = userId,
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result> UpdateStatusAsync(int id)
    {
        _logger.LogInformation("Method {method}, Id = {id}", nameof(UpdateStatusAsync), id);
        if (id <= 0)
            return Result.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo());
        var userId = _httpContext.GetCurrentUserId();
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.Deleted);

        if (category == null)
            return Result.Failure(ErrorCode.CategoryNotFound, this.GetCurrentMethodInfo());

        category.Deleted = true;
        category.ModifiedDate = DateTime.UtcNow;
        category.ModifiedBy = (int)userId;

        await _dbContext.Products
            .Where(p => p.CategoryId == id && !p.Deleted)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.CategoryId, p => null));

        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}