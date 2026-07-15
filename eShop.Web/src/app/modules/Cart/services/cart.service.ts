import { computed, Injectable, signal } from "@angular/core";
import { catchError, finalize, Observable, throwError, tap } from "rxjs";
import { AddToCartRequest, AddToCartResponse, ApiResult, CartErrorCode, CartItemDto, CartSummaryDto } from "../models/cart.models";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { environment } from "../../../my-lib/shared/enviroments/enviroment";


@Injectable({ providedIn: 'root' })
export class CartService {
    private readonly baseUrl = `${environment.api}/api/Cart`;

    private _totalItems = signal<number>(0);
    private _isSyncing = signal<boolean>(false);

    readonly totalItems = this._totalItems.asReadonly();
    readonly isSyncing = this._isSyncing.asReadonly();
    readonly hasItems = computed(() => this.totalItems() > 0);

    constructor(private http: HttpClient) { }

    addToCart(request: AddToCartRequest): Observable<ApiResult<AddToCartResponse>> {
        this._isSyncing.set(true);
        const previousCount = this._totalItems();
        this._totalItems.set(previousCount + request.quantity);

        return this.http.post<ApiResult<AddToCartResponse>>(
            `${this.baseUrl}/add-to-cart`,
            request
        ).pipe(
            tap((res) => {
                if (!res.isSuccess)
                    this._totalItems.set(previousCount);
            }),
            catchError((err: HttpErrorResponse) => {
                this._totalItems.set(previousCount);
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    getMyCart(): Observable<ApiResult<CartSummaryDto>> {
        this._isSyncing.set(true);
        return this.http.get<ApiResult<CartSummaryDto>>(`${this.baseUrl}/get-my-cart`).pipe(
            tap((res) => {
                if (res.isSuccess && res.value) {
                    this._totalItems.set(res.value.totalItems);
                }
            }),
            catchError((err: HttpErrorResponse) => {
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    updateCartItem(cartItemId: number, quantity: number): Observable<ApiResult<boolean>> {
        this._isSyncing.set(true);
        const payload = { cartItemId, quantity };
        return this.http.put<ApiResult<boolean>>(`${this.baseUrl}/update-item`, payload).pipe(
            catchError((err: HttpErrorResponse) => {
                return throwError(() => this.mapError(err));
            }),
            finalize(() => this._isSyncing.set(false))
        );
    }

    removeCartItem(cartItemId: number): Observable<ApiResult<boolean>> {
        this._isSyncing.set(true);
        return this.http.delete<ApiResult<boolean>>(`${this.baseUrl}/remove-item`, {
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
    private mapError(err: HttpErrorResponse): ApiResult<null> {
        const body = err.error as ApiResult<null> | undefined;
        if (body?.errorCode) {
            return body;
        }
        return {
            isSuccess: false,
            isFailure: true,
            errorCode: err.status === 401 ? CartErrorCode.Unauthorized : -1,
            otherData: 'Không thể kết nối máy chủ. Vui lòng thử lại.',
            value: null,
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