using CR.Core.ApplicationServices.CartModule.Dtos;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.CartModule.Abstracts;

public interface ICartService
{
    /// <summary>Lấy giỏ hàng hiện tại của user đang đăng nhập.</summary>
    Task<Result<CartDto>> GetCartAsync();

    /// <summary>Thêm sản phẩm vào giỏ hàng. Nếu đã có thì tăng số lượng.</summary>
    Task<Result<AddToCartDto>> AddItem(AddToCartDto input);

    /// <summary>Cập nhật số lượng của một CartItem.</summary>
    Task<Result<bool>> UpdateItem(UpdateCartItemDto input);

    /// <summary>Xóa một CartItem khỏi giỏ hàng.</summary>
    Task<Result<bool>> RemoveItem(int cartItemId);

    /// <summary>Xóa toàn bộ sản phẩm trong giỏ hàng.</summary>
    Task<Result<bool>> ClearCart();

    /// <summary> Validate giỏ hàng trước khi checkout : kiểm tra tồn kho , trạng thái sản phẩm. </summary>
    Task<Result<CartValidationResultDto>> ValidateCart();

    Task<Result<CheckoutPreviewDto>> ChechoutPerview(CheckoutPreviewRequestDto input);
}
