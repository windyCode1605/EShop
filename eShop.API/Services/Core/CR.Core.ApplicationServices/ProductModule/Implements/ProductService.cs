using AutoMapper;
using CR.ApplicationBase;
using CR.Common;
using CR.Core.ApplicationServices.ProductModule.Abstracts;
using CR.Core.Domain.Product;
using CR.Core.Domain.Category;
using CR.Core.Dtos.Product;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.Common.ServiceImplementations;

public class ProductService : ServiceBase<CoreDbContext>, IProductService
{
    public ProductService(CoreDbContext db,
        ILogger<ProductService> logger,
        IMapper mapper)
        : base(db, logger, mapper) { }

     public async Task<ProductResponseDto> CreateAsync(ProductRequestDto dto)
    {
        // Validate CategoryId exists
        var categoryExists = await _db.Set<Categories>()
            .AnyAsync(c => c.Id == dto.CategoryId);
        
        if (!categoryExists)
        {
            throw new InvalidOperationException($"Category with Id {dto.CategoryId} does not exist.");
        }

        var product = _mapper.Map<Products>(dto);

        product.Slug = GenerateSlug(dto.Name);

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        var result = await _db.Products
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
        var query = QueryActive<Products>()
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PaginatedResult<ProductResponseDto>
        {
            Items = _mapper.Map<List<ProductResponseDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = size
        };
    }
}