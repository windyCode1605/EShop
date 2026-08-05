import { Injectable, signal } from '@angular/core';
import { ApiResponse } from '../models';

export type ToastType = 'success' | 'error' | 'warning' | 'info';
export type ToastPosition = 'top-right' | 'top-left' | 'top-center' | 'bottom-right' | 'bottom-left' | 'bottom-center';

export interface ToastOptions {
  title?: string;
  duration?: number; // ms, default 4000
  position?: ToastPosition;
  dismissible?: boolean;
}

export interface ToastItem {
  id: string;
  type: ToastType;
  title?: string;
  message: string;
  duration: number;
  position: ToastPosition;
  dismissible: boolean;
  createdAt: number;
  statusCode?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  /** Signal chứa danh sách tất cả toast đang hiển thị */
  readonly toasts = signal<ToastItem[]>([]);

  private defaultPosition: ToastPosition = 'bottom-right';
  private defaultDuration = 4000;

  /**
   * Tự động trích xuất dữ liệu từ ApiResponse<T> để hiển thị Toast phù hợp.
   * Cấu trúc JSON API:
   * {
   *   "success": true,
   *   "data": true,
   *   "message": "Cập nhật quyền thành công",
   *   "errors": [],
   *   "statusCode": 200
   * }
   */
  showApiResponse<T>(response: ApiResponse<T>, options?: ToastOptions & { defaultSuccessMessage?: string; defaultErrorMessage?: string }): string {
    const isSuccess = response.success || (response.statusCode >= 200 && response.statusCode < 300);
    
    let msg = response.message;
    if (!msg) {
      if (!isSuccess && response.errors && response.errors.length > 0) {
        msg = response.errors.join(', ');
      } else {
        msg = isSuccess 
          ? (options?.defaultSuccessMessage || 'Thao tác thành công')
          : (options?.defaultErrorMessage || 'Đã xảy ra lỗi hệ thống');
      }
    }

    if (isSuccess) {
      return this.success(msg, options?.title || 'Thành công', { ...options });
    } else {
      return this.error(msg, options?.title || 'Thất bại', { ...options, statusCode: response.statusCode });
    }
  }

  /** Hiển thị Toast Thành công */
  success(message: string, title?: string, options?: ToastOptions & { statusCode?: number }): string {
    return this.add('success', message, title, options);
  }

  /** Hiển thị Toast Lỗi */
  error(message: string, title?: string, options?: ToastOptions & { statusCode?: number }): string {
    return this.add('error', message, title, options);
  }

  /** Hiển thị Toast Cảnh báo */
  warning(message: string, title?: string, options?: ToastOptions & { statusCode?: number }): string {
    return this.add('warning', message, title, options);
  }

  /** Hiển thị Toast Thông tin */
  info(message: string, title?: string, options?: ToastOptions & { statusCode?: number }): string {
    return this.add('info', message, title, options);
  }

  /** Tạo và đưa Toast vào danh sách hiển thị */
  private add(type: ToastType, message: string, title?: string, options?: ToastOptions & { statusCode?: number }): string {
    const id = 'toast_' + Math.random().toString(36).substring(2, 9) + '_' + Date.now();
    const duration = options?.duration ?? this.defaultDuration;
    const position = options?.position ?? this.defaultPosition;
    const dismissible = options?.dismissible ?? true;

    const newItem: ToastItem = {
      id,
      type,
      title,
      message,
      duration,
      position,
      dismissible,
      createdAt: Date.now(),
      statusCode: options?.statusCode
    };

    // Cập nhật signal: thêm toast mới vào mảng
    this.toasts.update(current => [...current, newItem]);

    // Tự động đóng sau khoảng thời gian duration nếu duration > 0
    if (duration > 0) {
      setTimeout(() => {
        this.dismiss(id);
      }, duration);
    }

    return id;
  }

  /** Đóng 1 toast theo id */
  dismiss(id: string): void {
    this.toasts.update(current => current.filter(t => t.id !== id));
  }

  /** Đóng tất cả toast */
  clear(): void {
    this.toasts.set([]);
  }
}
