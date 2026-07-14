using AutoMapper;
using CR.ApplicationBase;
using CR.Common;
using CR.Core.ApplicationServices.ProductModule.Abstracts;
using CR.Core.Domain.Catalog;
using CR.Core.Dtos.Product;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.Common.ServiceImplementations;

public class ProductService : ServiceBase<CoreDbContext>, IProductService
{
    public ProductService(
        CoreDbContext dbContext,
        ILogger<ProductService> logger,
        IMapper mapper)
        : base(dbContext, logger, mapper) { }

    public async Task<ProductResponseDto> CreateAsync(ProductRequestDto dto)
    {
        // Validate CategoryId exists
        var categoryExists = await _dbContext.Set<Category>()
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException($"Category with Id {dto.CategoryId} does not exist.");
        }

        var product = _mapper.Map<Product>(dto);

        product.Slug = GenerateSlug(dto.Name);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var result = await _dbContext.Products
            .Include(p => p.Category)
            .FirstAsync(p => p.Id == product.Id);

        _logger.LogInformation("Product created: Id={Id}, Name={Name}", result.Id, result.Name);

        return _mapper.Map<ProductResponseDto>(result);
    }

    private string GenerateSlug(string name)
    {
        return name.ToLower()
                   .Replace(" ", "-")
                   .Replace(".", "")
                   .Replace(",", "");
    }

    public async Task<PaginatedResult<ProductResponseDto>> GetAllAsync(int page, int size)
    {
        _logger.LogInformation("Method Name: {Method}, Page: {Page}, Size: {Size}", nameof(GetAllAsync), page, size);
        var query = _dbContext.Products
            .Where(p => !p.Deleted)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.VariantAttributes.Where(va => !va.Deleted))
                    .ThenInclude(va => va.Attribute)
            .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.VariantAttributes.Where(va => !va.Deleted))
                    .ThenInclude(va => va.AttributeValue)
            .OrderByDescending(p => p.CreatedDate);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsSplitQuery()
            .ToListAsync();

        return new PaginatedResult<ProductResponseDto>
        {
            Items = _mapper.Map<List<ProductResponseDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = size
        };
    }
    // public async Task<Result<ProductResponseDto>> UpdateAsync(ProductRequestDto input)
    // {

    // }
}
