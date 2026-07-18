using AutoMapper;
using CR.ApplicationBase;
using CR.Common;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.ProductModule.Abstracts;
using CR.Core.Domain.Catalog;
using CR.Core.Dtos.Product;
using CR.DtoBase;
using CR.Utils.DataUtils;
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
            .Include(p => p.Variants)
            .FirstAsync(p => p.Id == product.Id);

        _logger.LogInformation("Product created: Id={Id}, Name={Name}, Variants={Count}",
            result.Id, result.Name, result.Variants.Count);

        return _mapper.Map<ProductResponseDto>(result);
    }

    private string GenerateSlug(string name)
    {
        return name.ToLower()
                   .Replace(" ", "-")
                   .Replace(".", "")
                   .Replace(",", "");
    }

    public async Task<PaginatedResult<ProductResponseDto>> GetAllAsync(int page, int size, int? categoryId = null)
    {
        _logger.LogInformation("Method Name: {Method}, Page: {Page}, Size: {Size}, CategoryId: {CategoryId}", nameof(GetAllAsync), page, size, categoryId);
        var query = _dbContext.Products
            .Where(p => !p.Deleted)
            .Where(p => !categoryId.HasValue || p.CategoryId == categoryId)
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
    public async Task<Result<ProductResponseDto>> GetByIdAsync(int id)
    {
        _logger.LogInformation("Method Name: {method}", nameof(GetByIdAsync));
        var product = await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.ProductAttributes.Where(pa => !pa.Deleted))
                .ThenInclude(pa => pa.Attribute)
            .Include(p => p.ProductAttributes.Where(pa => !pa.Deleted))
                .ThenInclude(pa => pa.AttributeValue)
            .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.VariantAttributes.Where(va => !va.Deleted))
                    .ThenInclude(va => va.Attribute)
            .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.VariantAttributes.Where(va => !va.Deleted))
                    .ThenInclude(av => av.AttributeValue)
            .FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);

        if (product == null)
        {
            return Result<ProductResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Product not found");
        }

        var resultDto = _mapper.Map<ProductResponseDto>(product);
        return Result<ProductResponseDto>.Success(resultDto);
    }

    public async Task<Result<ProductVariantResponseDto>> CreateProductVariantAsync(CreateProductVariantDto dto)
    {
        _logger.LogInformation("Method Name: {Method}, ProductId={ProductId}, SKU={SKU}",
            nameof(CreateProductVariantAsync), dto.ProductId, dto.SKU);

        if (dto.ProductId is null or <= 0)
            return Result<ProductVariantResponseDto>.Failure(
                ErrorCode.InvalidInput, this.GetCurrentMethodInfo(),
                "ProductId is required.");
        var productExists = await _dbContext.Products
            .AnyAsync(p => p.Id == dto.ProductId && !p.Deleted);

        if (!productExists)
            return Result<ProductVariantResponseDto>.Failure(
                ErrorCode.InvalidInput, this.GetCurrentMethodInfo(),
                $"Product with Id {dto.ProductId} does not exist.");

        var skuExists = await _dbContext.ProductVariants
            .AnyAsync(v => v.SKU == dto.SKU && !v.Deleted);

        if (skuExists)
            return Result<ProductVariantResponseDto>.Failure(
                ErrorCode.InvalidInput, this.GetCurrentMethodInfo(),
                $"SKU '{dto.SKU}' already exists.");

        var variant = _mapper.Map<ProductVariant>(dto);
        variant.ProductId = dto.ProductId.Value;

        _dbContext.ProductVariants.Add(variant);
        await _dbContext.SaveChangesAsync();

        var saved = await _dbContext.ProductVariants
            .Include(v => v.VariantAttributes.Where(va => !va.Deleted))
                .ThenInclude(va => va.Attribute)
            .Include(v => v.VariantAttributes.Where(va => !va.Deleted))
                .ThenInclude(va => va.AttributeValue)
            .FirstAsync(v => v.Id == variant.Id);

        _logger.LogInformation("ProductVariant created: Id={Id}, SKU={SKU}, ProductId={ProductId}",
            saved.Id, saved.SKU, saved.ProductId);

        return Result<ProductVariantResponseDto>.Success(
            _mapper.Map<ProductVariantResponseDto>(saved));
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int id, ProductRequestDto dto)
    {
        _logger.LogInformation("Method Name: {Method}, ProductId={ProductId}", nameof(UpdateAsync), id);

        var product = await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);

        if (product == null)
        {
            return Result<ProductResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Product not found");
        }

        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId && !c.Deleted);
        if (!categoryExists)
        {
            return Result<ProductResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), $"Category with Id {dto.CategoryId} does not exist.");
        }

        // Cập nhật thông tin cơ bản
        product.Name = dto.Name;
        product.CategoryId = dto.CategoryId;
        product.BasePrice = dto.Price;
        product.Description = dto.Description;
        product.Slug = GenerateSlug(dto.Name);

        // UpdateAsync này tạm thời CHỈ cập nhật thông tin Product cơ bản. 
        // Các Variants được quản lý độc lập qua luồng POST /api/Product/variants, 
        // hoặc  có thể gọi endpoint cập nhật/xóa variant riêng.

        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();

        var resultDto = _mapper.Map<ProductResponseDto>(product);
        return Result<ProductResponseDto>.Success(resultDto);
    }
}
