import { computed, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_ENDPOINTS } from '../../core/constants/api-endpoints.const';

export interface UserAuthorization {
  roles: string[];
  permissions: string[];
}

/**
 * PermissionStore — Signal-based store quản lý Roles & Permissions của user.
 *
 * Luồng:
 *   1. User đăng nhập → /connect/token → access_token (chứa roles trong JWT)
 *   2. AppAuthService gọi permissionStore.load() → fetch /api/me/permissions
 *   3. Store lưu vào signal → toàn app reactive theo
 *
 * Sử dụng:
 *   - permissionStore.hasPermission('Products.Create') → boolean
 *   - permissionStore.hasRole('Admin') → boolean
 *   - permissionStore.roles() → string[]
 */
@Injectable({ providedIn: 'root' })
export class PermissionStore {

  private readonly _roles = signal<string[]>([]);
  private readonly _permissions = signal<string[]>([]);
  private readonly _loaded = signal(false);

  readonly roles = this._roles.asReadonly();
  readonly permissions = this._permissions.asReadonly();
  readonly isLoaded = this._loaded.asReadonly();

  readonly isAdmin = computed(() =>
    this._roles().some(r => r.toLowerCase() === 'admin' || r.toLowerCase() === 'super_admin')
  );

  constructor(private readonly http: HttpClient) { }

  /**
   * Fetch roles + permissions từ /api/me/permissions
   * Gọi sau khi đăng nhập thành công.
   */
  async load(): Promise<void> {
    try {
      const res = await firstValueFrom(
        this.http.get<{ data: UserAuthorization }>(API_ENDPOINTS.ME.PERMISSIONS)
      );
      this._roles.set(res.data.roles ?? []);
      this._permissions.set(res.data.permissions ?? []);
      this._loaded.set(true);
    } catch (err) {
      console.warn('[PermissionStore] Không thể load permissions:', err);
      this._roles.set([]);
      this._permissions.set([]);
      this._loaded.set(false);
    }
  }

  /**
   * Kiểm tra user có permission cụ thể không.
   * @example permissionStore.hasPermission('Products.Create')
   */
  hasPermission(key: string): boolean {
    return this._permissions().some(p => p.toLowerCase() === key.toLowerCase());
  }

  /**
   * Kiểm tra user có role cụ thể không.
   * @example permissionStore.hasRole('Admin')
   */
  hasRole(role: string): boolean {
    return this._roles().some(r => r.toLowerCase() === role.toLowerCase());
  }

  /**
   * Reset store khi logout.
   */
  clear(): void {
    this._roles.set([]);
    this._permissions.set([]);
    this._loaded.set(false);
  }
}
