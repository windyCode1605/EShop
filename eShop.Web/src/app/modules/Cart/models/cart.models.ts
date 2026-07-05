export interface AddToCartRequest {
    productVariantId: number;
    quantity: number;
}
export interface ApiResult<T> {
    isSuccess: boolean;
    isFailure: boolean;
    errorCode: number;
    otherData?: string;
    listParam?: string[];
    stackTrace?: string;
    value: T;
}
export interface CartItemDto {
    cartItemId: number;
    productVariantId: number;
    productName: string;
    variantSKU: string;
    size?: string;
    color?: string;
    quantity: number;
    unitPrice: number;
    lineTotal: number;
    stockQuantity: number;
    imageUrl?: string;
}
export interface CartSummaryDto {
    cartId: number;
    items: CartItemDto[];
    totalItems: number;
    subtotal: number;
}

export interface AddToCartResponse {
    productVariantId: number;
    quantity: number;
}

/** Mã lỗi nghiệp vụ — nên đồng bộ với enum ErrorCode bên BE */
export enum CartErrorCode {
    None = 0,
    OutOfStock = 1001,
    VariantNotFound = 1002,
    InvalidQuantity = 1003,
    ExceedStockLimit = 1004,
    Unauthorized = 401,
}