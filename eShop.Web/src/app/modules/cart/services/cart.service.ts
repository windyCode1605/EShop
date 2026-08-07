import { computed, Injectable, signal } from "@angular/core";
import { catchError, finalize, Observable, throwError, tap } from "rxjs";
import { AddToCartRequest, AddToCartResponse, CartErrorCode, CartItemDto, CartSummaryDto } from "../models/cart.models";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { API_ENDPOINTS } from "../../../core/constants/api-endpoints.const";
import { ApiResponse } from "../../../core/models";


@Injectable({ providedIn: 'root' })
export class CartService {
    private _totalItems = signal<number>(0);
    private _isSyncing = signal<boolean>(false);

    readonly totalItems = this._totalItems.asReadonly();
    readonly isSyncing = this._isSyncing.asReadonly();
    readonly hasItems = computed(() => this.totalItems() > 0);

    constructor(private http: HttpClient) { }

    addToCart(request: AddToCartRequest): Observable<ApiResponse<AddToCartResponse>> {
        this._isSyncing.set(true);
        const previousCount = this._totalItems();
        this._totalItems.set(previousCount + request.quantity);

        return this.http.post<ApiResponse<AddToCartResponse>>(
            API_ENDPOINTS.CART.ADD_TO_CART,
            request
        ).pipe(
            tap((res) => {
                if (!res.success)
                    this._totalItems.set(previousCount);
            }),
            catchError((err: HttpErrorResponse) => {
                this._totalItems.set(previousCount);
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    getMyCart(): Observable<ApiResponse<CartSummaryDto>> {
        this._isSyncing.set(true);
        return this.http.get<ApiResponse<CartSummaryDto>>(API_ENDPOINTS.CART.GET_MY_CART).pipe(
            tap((res) => {
                if (res.success && res.data) {
                    this._totalItems.set(res.data.totalItems);
                }
            }),
            catchError((err: HttpErrorResponse) => {
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    updateCartItem(cartItemId: number, quantity: number): Observable<ApiResponse<boolean>> {
        this._isSyncing.set(true);
        const payload = { cartItemId, quantity };
        return this.http.put<ApiResponse<boolean>>(API_ENDPOINTS.CART.UPDATE_ITEM, payload).pipe(
            catchError((err: HttpErrorResponse) => {
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    removeCartItem(cartItemId: number): Observable<ApiResponse<boolean>> {
        this._isSyncing.set(true);
        return this.http.delete<ApiResponse<boolean>>(API_ENDPOINTS.CART.REMOVE_ITEM, {
            body: cartItemId,
            headers: { 'Content-Type': 'application/json' }
        }).pipe(
            catchError((err: HttpErrorResponse) => {
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    /** Chuẩn hoá lỗi để component không phải parse HttpErrorResponse thủ công */
    private mapError(err: HttpErrorResponse): ApiResponse<null> {
        const body = err.error as ApiResponse<null> | undefined;
        if (body && !body.success) {
            return body;
        }
        return {
            success: false,
            statusCode: err.status === 401 ? CartErrorCode.Unauthorized : 500,
            message: 'Không thể kết nối máy chủ. Vui lòng thử lại.',
            errors: []
        };
    }


    getErrorMessage(errorCode: number, fallback?: string): string {
        switch (errorCode) {
            case CartErrorCode.OutOfStock:
                return 'Sản phẩm đã hết hàng.';
            case CartErrorCode.VariantNotFound:
                return 'Phiên bản sản phẩm không tồn tại.';
            case CartErrorCode.ExceedStockLimit:
                return 'Số lượng vượt quá tồn kho hiện có.';
            case CartErrorCode.InvalidQuantity:
                return 'Số lượng không hợp lệ.';
            case CartErrorCode.Unauthorized:
                return 'Vui lòng đăng nhập để thêm vào giỏ hàng.';
            default:
                return fallback ?? 'Đã có lỗi xảy ra. Vui lòng thử lại.';
        }
    }
}