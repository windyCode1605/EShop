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
using CR.Utils.Helpers;
using CR.ApplicationBase.Common;
using CR.Core.ApplicationServices.ProductModule.Implements;

namespace CR.Core.ApplicationServices.Common.ServiceImplementations;

public class ProductService : ServiceBase<CoreDbContext>, IProductService
{
    public ProductService(
        CoreDbContext dbContext,
        ILogger<ProductService> logger,
        IMapper mapper)
        : base(dbContext, logger, mapper) { }

    public async Task<Result<ProductResponseDto>> CreateAsync(ProductRequestDto dto)
    {
        // Validate CategoryId exists
        var categoryExists = await _dbContext.Set<Category>()
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return Result<ProductResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), $"Category with Id {dto.CategoryId} does not exist.");
        }

        var product = _mapper.Map<Product>(dto);

        if (dto.ImageUrls != null && dto.ImageUrls.Any())
        {
            product.Images = new List<ProductImage>();

            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                product.Images.Add(new ProductImage
                {
                    Url = dto.ImageUrls[i],
                    SortOrder = i,
                    IsPrimary = (i == 0)
                });
            }
        }
        product.Slug = dto.Name.ToSlug();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var result = await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .FirstAsync(p => p.Id == product.Id);

        _logger.LogInformation("Product created: Id={Id}, Name={Name}, Variants={Count}",
            result.Id, result.Name, result.Variants.Count);

        return Result<ProductResponseDto>.Success(_mapper.Map<ProductResponseDto>(result));
    }

    public async Task<Result<PageResult<ProductResponseDto>>> GetAllAsync(ProductQueryDto request)
    {
        _logger.LogInformation("Method Name: {Method}, Keyword: {Keyword}, CategoryId: {CategoryId}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}", 
            nameof(GetAllAsync), request.Keyword, request.CategoryId, request.MinPrice, request.MaxPrice);
            
        var query = _dbContext.Products
            .Where(p => !p.Deleted);
            
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(keyword) || p.Slug.Contains(keyword));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.BasePrice >= request.MinPrice);
            
        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= request.MaxPrice);

        query = query.IncludeFullProductDetails();

        var total = await query.CountAsync();

        var items = await query
            .PagingAndSorting(request)
            .AsSplitQuery()
            .ToListAsync();

        var dtos = _mapper.Map<List<ProductResponseDto>>(items);
        return Result<PageResult<ProductResponseDto>>.Success(PageResult<ProductResponseDto>.Create(dtos, total, request));
    }
    public async Task<Result<ProductResponseDto>> GetByIdAsync(int id)
    {
        _logger.LogInformation("Method Name: {method}", nameof(GetByIdAsync));
        var product = await _dbContext.Products
            .IncludeFullProductDetails()
            .Include(p => p.ProductAttributes.Where(pa => !pa.Deleted))
                .ThenInclude(pa => pa.Attribute)
            .Include(p => p.ProductAttributes.Where(pa => !pa.Deleted))
                .ThenInclude(pa => pa.AttributeValue)
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

    public async Task<Result<ProductVariantResponseDto>> UpdateProductVariantAsync(int variantId, UpdateProductVariantDto dto)
    {
        _logger.LogInformation("Method Name: {Method}, VariantId={VariantId}", nameof(UpdateProductVariantAsync), variantId);

        var variant = await _dbContext.ProductVariants
            .Include(v => v.Images)
            .Include(v => v.VariantAttributes.Where(va => !va.Deleted))
                .ThenInclude(va => va.Attribute)
            .Include(v => v.VariantAttributes.Where(va => !va.Deleted))
                .ThenInclude(va => va.AttributeValue)
            .FirstOrDefaultAsync(v => v.Id == variantId && !v.Deleted);

        if (variant == null)
            return Result<ProductVariantResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Variant not found");

        var skuExists = await _dbContext.ProductVariants
            .AnyAsync(v => v.SKU == dto.SKU && v.Id != variantId && !v.Deleted);

        if (skuExists)
            return Result<ProductVariantResponseDto>.Failure(
                ErrorCode.InvalidInput, this.GetCurrentMethodInfo(),
                $"SKU '{dto.SKU}' already exists.");

        variant.SKU = dto.SKU;
        variant.PriceAdjustment = dto.PriceAdjustment;
        variant.StockQuantity = dto.StockQuantity;

        if (variant.Images != null && variant.Images.Any())
        {
            _dbContext.RemoveRange(variant.Images);
            variant.Images.Clear();
        }

        if (dto.ImageUrls != null && dto.ImageUrls.Any())
        {
            variant.Images = new List<ProductImage>();
            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                variant.Images.Add(new ProductImage
                {
                    ProductId = variant.ProductId,
                    Url = dto.ImageUrls[i],
                    SortOrder = i,
                    IsPrimary = (i == 0)
                });
            }
        }

        _dbContext.ProductVariants.Update(variant);
        await _dbContext.SaveChangesAsync();

        var resultDto = _mapper.Map<ProductVariantResponseDto>(variant);
        return Result<ProductVariantResponseDto>.Success(resultDto);
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int id, ProductRequestDto dto)
    {
        _logger.LogInformation("Method Name: {Method}, ProductId={ProductId}", nameof(UpdateAsync), id);

        var product = await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
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
        product.Slug = dto.Name.ToSlug();

        // UpdateAsync này tạm thời CHỈ cập nhật thông tin Product cơ bản. 
        // Các Variants được quản lý độc lập qua luồng POST /api/Product/variants, 
        // hoặc  có thể gọi endpoint cập nhật/xóa variant riêng.

        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();

        var resultDto = _mapper.Map<ProductResponseDto>(product);
        return Result<ProductResponseDto>.Success(resultDto);
    }
}
