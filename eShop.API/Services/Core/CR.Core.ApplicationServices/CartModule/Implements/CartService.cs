using CR.ApplicationBase.Localization;
using CR.Constants.Discount;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.CartModule.Abstracts;
using CR.Core.ApplicationServices.CartModule.Dtos;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.Carts;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;


namespace CR.Core.ApplicationServices.CartModule.Implemts;

public class CartService : CoreServiceBase, ICartService
{
    public CartService(ILogger<CartService> logger, IHttpContextAccessor httpContext)
    : base(logger, httpContext)
    {
    }

    public async Task<Result<CartDto>> GetCartAsync()
    {
        _logger.LogInformation("{method} called", nameof(GetCartAsync));
        var userId = _httpContext.GetCurrentUserId();

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Images.Where(i => i.IsPrimary && !i.Deleted))
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.Deleted);

        if (cart == null)
        {
            return Result<CartDto>.Success(new CartDto());
        }

        var cartDto = MapToCartDto(cart);
        return Result<CartDto>.Success(cartDto);
    }

    public async Task<Result<AddToCartDto>> AddItem(AddToCartDto input)
    {
        _logger.LogInformation("{method} called with input: {@input}", nameof(AddItem), input);
        var userId = _httpContext.GetCurrentUserId();

        var variant = await _dbContext.ProductVariants
            .Include(v => v.Product)
                .ThenInclude(p => p.Images.Where(i => i.IsPrimary && !i.Deleted))
            .FirstOrDefaultAsync(v =>
                v.Id == input.ProductVariantId &&
                !v.Deleted &&
                !v.Product.Deleted);

        if (variant == null)
        {
            return Result<AddToCartDto>.Failure(
                ErrorCode.ProductVariantNotFound, this.GetCurrentMethodInfo(),
                "Product variant not found"
            );
        }

        if (variant.StockQuantity <= 0)
        {
            return Result<AddToCartDto>.Failure(
                ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(),
                "Product is out of stock"
            );
        }

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.Deleted);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };
            _dbContext.Carts.Add(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == input.ProductVariantId);
        if (existingItem != null)
        {
            var newQty = existingItem.Quantity + input.Quantity;
            if (newQty > variant.StockQuantity)
            {
                return Result<AddToCartDto>.Failure(
                    ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(),
                    $"Only {variant.StockQuantity} items available in stock"
                );
            }
            existingItem.Quantity = newQty;
            existingItem.ModifiedDate = DateTime.UtcNow;
        }
        else
        {
            if (input.Quantity > variant.StockQuantity)
            {
                return Result<AddToCartDto>.Failure(
                    ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(),
                    $"Only {variant.StockQuantity} items available in stock"
                );
            }
            cart.Items.Add(new CartItem
            {
                ProductVariantId = input.ProductVariantId,
                Quantity = input.Quantity,
                CreatedDate = DateTime.UtcNow
            });
        }

        cart.LastUpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Result<AddToCartDto>.Success(input);
    }

    public async Task<Result<bool>> UpdateItem(UpdateCartItemDto input)
    {
        _logger.LogInformation("{method} called with input: {@input}", nameof(UpdateItem), input);
        var userId = _httpContext.GetCurrentUserId();

        var cartItem = await _dbContext.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.ProductVariant)
            .FirstOrDefaultAsync(ci =>
                ci.Id == input.CartItemId &&
                ci.Cart.UserId == userId &&
                !ci.Deleted);

        if (cartItem == null)
        {
            return Result<bool>.Failure(ErrorCode.CartEmpty, this.GetCurrentMethodInfo(), "Cart item not found");
        }

        if (input.Quantity <= 0)
        {
            cartItem.Deleted = true;
            cartItem.DeletedDate = DateTime.UtcNow;
        }
        else
        {
            if (input.Quantity > cartItem.ProductVariant.StockQuantity)
            {
                return Result<bool>.Failure(
                    ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(),
                    $"Only {cartItem.ProductVariant.StockQuantity} items available"
                );
            }
            cartItem.Quantity = input.Quantity;
            cartItem.ModifiedDate = DateTime.UtcNow;
        }

        cartItem.Cart.LastUpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveItem(int cartItemId)
    {
        _logger.LogInformation("{method} called with cartItemId: {id}", nameof(RemoveItem), cartItemId);
        var userId = _httpContext.GetCurrentUserId();

        var cartItem = await _dbContext.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci =>
                ci.Id == cartItemId &&
                ci.Cart.UserId == userId &&
                !ci.Deleted);

        if (cartItem == null)
        {
            return Result<bool>.Failure(ErrorCode.CartEmpty, this.GetCurrentMethodInfo(), "Cart item not found");
        }

        cartItem.Deleted = true;
        cartItem.DeletedDate = DateTime.UtcNow;
        cartItem.Cart.LastUpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ClearCart()
    {
        _logger.LogInformation("{method} called", nameof(ClearCart));
        var userId = _httpContext.GetCurrentUserId();

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.Deleted);

        if (cart == null || cart.Items.Count == 0)
        {
            return Result<bool>.Success(true);
        }

        foreach (var item in cart.Items)
        {
            item.Deleted = true;
            item.DeletedDate = DateTime.UtcNow;
        }

        cart.LastUpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<CartValidationResultDto>> ValidateCart()
    {
        _logger.LogInformation("{method} called", nameof(ValidateCart));
        var userId = _httpContext.GetCurrentUserId();

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.Deleted);
        if (cart == null || cart.Items.Count == 0)
            return Result<CartValidationResultDto>.Failure(
                ErrorCode.CartEmpty, this.GetCurrentMethodInfo(), "Cart is empty or not found."
            );

        var issues = new List<CartItemIssueDto>();

        foreach (var item in cart.Items)
        {
            var pv = item.ProductVariant;

            if (pv.Deleted || pv.Product.Deleted)
            {
                issues.Add(new CartItemIssueDto
                {
                    CartItemId = item.Id,
                    ProductName = pv.Product.Name,
                    SKU = pv.SKU,
                    IssueType = "OUT_OF_STOCK",
                    Message = "Sản phẩm không còn được bán.",
                    AvailableQuantity = 0,
                });
                continue;
            }
            else if (item.Quantity > pv.StockQuantity)
            {
                issues.Add(new CartItemIssueDto
                {
                    CartItemId = item.Id,
                    ProductName = pv.Product.Name,
                    SKU = pv.SKU,
                    IssueType = item.Quantity == 0 ? "INSUFFICIENT_STOCK" : "INSUFFICIENT_STOCK",
                    Message = item.Quantity == 0
                        ? "Sản phẩm đã hết hàng."
                        : $"chỉ còn {pv.StockQuantity} sản phẩm {pv.Product.Name}",
                    AvailableQuantity = pv.StockQuantity
                });
            }
            else if (pv.StockQuantity <= 0)
            {
                issues.Add(new CartItemIssueDto
                {
                    CartItemId = item.Id,
                    ProductName = pv.Product.Name,
                    SKU = pv.SKU,
                    IssueType = "OUT_OF_STOCK",
                    Message = "Hết hàng",
                    AvailableQuantity = 0,
                });
            }
        }
        return Result<CartValidationResultDto>.Success(new
        CartValidationResultDto
        {
            IsValid = issues.Count == 0,
            Issues = issues
        });
    }
    public async Task<Result<CheckoutPreviewDto>> ChechoutPerview(CheckoutPreviewRequestDto input)
    {
        _logger.LogInformation("{method} called with input: {@input}", nameof(ChechoutPerview), input);
        var userId = _httpContext.GetCurrentUserId();
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .ThenInclude(p => p.Images.Where(i => i.IsPrimary && !i.Deleted))
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.Deleted);
        if (cart == null || cart.Items.Count == 0)
            return Result<CheckoutPreviewDto>.Failure(
                ErrorCode.CartEmpty, this.GetCurrentMethodInfo(), "Cart is empty or not found."
            );

        var cartDto = MapToCartDto(cart);
        var Subtotal = cartDto.Subtotal;
        decimal discountAmount = 0;
        string? couponMessage = null;

        // Tính discount nếu có coupon 
        if (!string.IsNullOrWhiteSpace(input.CouponCode))
        {
            var now = DateTime.UtcNow;
            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(c =>
                    c.Code == input.CouponCode &&
                    c.IsActive &&
                    !c.IsDeleted &&
                    c.StartDate <= now &&
                    c.ExpiryDate >= now &&
                    (c.UsageLimit == null || c.UsedCount < c.UsageLimit)
                );
            if (coupon == null)
                return Result<CheckoutPreviewDto>.Failure(
                    ErrorCode.CouponNotFound, this.GetCurrentMethodInfo(), "Coupon not found."
                );
            if (coupon.MinOrderValue.HasValue && Subtotal < coupon.MinOrderValue)
                return Result<CheckoutPreviewDto>.Failure(
                    ErrorCode.CouponMinOrderNotMet, this.GetCurrentMethodInfo(),
                    $"Đơn hàng tối thiểu {coupon.MaxDiscountValue:N0}d ddeer áp dụng mã này");
            // tính dicount
            if (coupon.DiscountType == DiscountType.Percentage)
            {
                discountAmount = Subtotal * coupon.DiscountValue / 100;
                if (coupon.MaxDiscountValue.HasValue)
                    discountAmount = Math.Min(discountAmount, coupon.MaxDiscountValue.Value);
                couponMessage = $"Giảm {coupon.DiscountValue}% ({Math.Round(discountAmount, 0):N0}đ)";
            }
            else
            {
                discountAmount = Math.Min(coupon.DiscountValue, Subtotal);
                couponMessage = $"Giảm {coupon.DiscountValue:N0}đ";
            }
        }
        var total = Subtotal + input.ShippingFee - discountAmount;
        return Result<CheckoutPreviewDto>.Success(new
            CheckoutPreviewDto
        {
            Subtotal = Subtotal,
            ShippingFee = input.ShippingFee,
            DiscountAmount = discountAmount,
            CouponCode = input.CouponCode,
            CouponMessage = couponMessage,
            Total = total,
            Items = cartDto.Items
        });
    }

    // ─── Private Helpers ───────────────────────────────────────────────────────

    private static CartDto MapToCartDto(Cart cart)
    {
        var items = cart.Items.Select(ci =>
        {
            var unitPrice = ci.ProductVariant.Product.BasePrice + ci.ProductVariant.PriceAdjustment;
            return new CartItemDto
            {
                Id = ci.Id,
                ProductVariantId = ci.ProductVariantId,
                ProductId = ci.ProductVariant.ProductId,
                ProductName = ci.ProductVariant.Product.Name,
                SKU = ci.ProductVariant.SKU,
                Attributes = ci.ProductVariant.VariantAttributes
                    .Where(va => !va.Deleted)
                    .Select(va => new CR.Core.Dtos.Product.VariantAttributeDto
                    {
                        AttributeId = va.AttributeId,
                        AttributeName = va.Attribute?.Name ?? string.Empty,
                        AttributeType = va.Attribute?.AttributeType ?? string.Empty,
                        AttributeValueId = va.AttributeValueId,
                        AttributeValue = va.AttributeValue?.Value,
                        CustomValue = va.CustomValue
                    }).ToList(),
                UnitPrice = unitPrice,
                Quantity = ci.Quantity,
                LineTotal = unitPrice * ci.Quantity,
                ImageUrl = ci.ProductVariant.Product.Images.FirstOrDefault()?.Url,
                IsAvailable = !ci.Deleted && !ci.ProductVariant.Deleted && ci.ProductVariant.StockQuantity > 0,
                MaxQuantity = ci.ProductVariant.StockQuantity
            };
        }).ToList();

        return new CartDto
        {
            Id = cart.Id,
            LastUpdatedAt = cart.LastUpdatedAt,
            TotalItems = items.Sum(i => i.Quantity),
            Subtotal = items.Sum(i => i.LineTotal),
            Items = items
        };
    }
}