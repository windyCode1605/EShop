using AutoMapper;
using CR.ApplicationBase;
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
using CR.InfrastructureBase;


namespace CR.Core.ApplicationServices.Common.ServiceImplementations;

public class ProductService : ServiceBase<CoreDbContext>, IProductService
{
    public ProductService(
        CoreDbContext dbContext,
        ILogger<ProductService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContext)
        : base(dbContext, logger, mapper, httpContext) { }

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

        await _dbContext.Entry(product).Reference(p => p.Category).LoadAsync();

        _logger.LogInformation("Product created: Id={Id}, Name={Name}",
            product.Id, product.Name);

        return Result<ProductResponseDto>.Success(_mapper.Map<ProductResponseDto>(product));
    }

    public async Task<Result<PageResult<ProductResponseDto>>> GetAllAsync(ProductQueryDto request)
    {
        _logger.LogInformation("Method Name: {Method}, Keyword: {Keyword}, CategoryId: {CategoryId}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
            nameof(GetAllAsync), request.Keyword, request.CategoryId, request.MinPrice, request.MaxPrice);

        var query = _dbContext.Products
            .AsNoTracking()
            .Where(p => !p.Deleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            var searchPattern = $"%{keyword}%";

            query = query.Where(p =>
                EF.Functions.ILike(p.Name, searchPattern) ||
                EF.Functions.ILike(p.Slug, searchPattern));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.BasePrice >= request.MinPrice);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.BasePrice <= request.MaxPrice);

        var total = await query.CountAsync();
        if (total == 0)
        {
            return Result<PageResult<ProductResponseDto>>.Success(
                PageResult<ProductResponseDto>.Create(new List<ProductResponseDto>(), 0, request));
        }
        var items = await query
            .IncludeFullProductDetails()
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
            .AsSplitQuery()
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
            .AsSplitQuery()
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

        if (variant.Images == null)
        {
            variant.Images = new List<ProductImage>();
        }
        else if (variant.Images.Any())
        {
            _dbContext.ProductImages.RemoveRange(variant.Images);
            variant.Images.Clear();
        }

        if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
        {
            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                variant.Images.Add(new ProductImage
                {
                    ProductId = variant.ProductId,
                    ProductVariantId = variant.Id,
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

        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId && !c.Deleted);
        if (!categoryExists)
        {
            return Result<ProductResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), $"Category with Id {dto.CategoryId} does not exist.");
        }


        var product = await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);

        if (product == null)
        {
            return Result<ProductResponseDto>.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), "Product not found");
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
    public async Task<Result> DeleteAsync(int id)
    {
        _logger.LogInformation("Method : {method}, Product ID : {id}", nameof(DeleteAsync), id);
        var userId = _httpContext.GetCurrentUserId();

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;

            // Dùng ExecuteUpdateAsync thay vì load toàn bộ graph vào memory (Tối ưu truy vấn N+1 & Memory)
            var rowsAffected = await _dbContext.Products
                .Where(p => p.Id == id && !p.Deleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Deleted, true)
                    .SetProperty(p => p.ModifiedDate, now)
                    .SetProperty(p => p.DeletedDate, now)
                    .SetProperty(p => p.DeletedBy, userId));

            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return Result.Failure(ErrorCode.InvalidInput, this.GetCurrentMethodInfo(), $"Không tìm thấy sản phẩm với ID : {id}");
            }

            // Xóa ProductAttributes liên quan
            await _dbContext.ProductAttributes
                .Where(pa => pa.ProductId == id && !pa.Deleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Deleted, true)
                    .SetProperty(p => p.DeletedDate, now)
                    .SetProperty(p => p.DeletedBy, userId));

            // Xóa ProductImages (Bao gồm ảnh của product và ảnh của các variants do đều map tới ProductId)
            await _dbContext.ProductImages
                .Where(pi => pi.ProductId == id && !pi.Deleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Deleted, true)
                    .SetProperty(p => p.DeletedDate, now)
                    .SetProperty(p => p.DeletedBy, userId));

            // Xóa ProductVariantAttributes thông qua relationship với ProductVariant
            await _dbContext.ProductVariantAttributes
                .Where(pva => pva.ProductVariant.ProductId == id && !pva.Deleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Deleted, true)
                    .SetProperty(p => p.DeletedDate, now)
                    .SetProperty(p => p.DeletedBy, userId));

            // Xóa các ProductVariants
            await _dbContext.ProductVariants
                .Where(pv => pv.ProductId == id && !pv.Deleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Deleted, true)
                    .SetProperty(p => p.DeletedDate, now)
                    .SetProperty(p => p.DeletedBy, userId));

            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi khi xóa Product ID:{id}", id);
            return Result.Failure(ErrorCode.InternalServerError, this.GetCurrentMethodInfo(), "Lỗi hệ thống khi xóa sản phẩm.");
        }
    }
}
