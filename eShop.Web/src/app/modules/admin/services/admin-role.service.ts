import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { ApiResponse } from '../../../core/models';
import { IRole, ICreateRoleRequest, IPermissionItem, IPermissionGroup } from '../models/admin-role.model';

@Injectable({ providedIn: 'root' })
export class AdminRoleService {
  // Signals for state management
  roles = signal<IRole[]>([]);
  allPermissionsGrouped = signal<IPermissionGroup[]>([]);
  loading = signal<boolean>(false);
  
  constructor(private http: HttpClient) {}

  /**
   * Get all active roles
   */
  fetchRoles(): Observable<IRole[]> {
    this.loading.set(true);
    return this.http.get<ApiResponse<IRole[]>>(API_ENDPOINTS.ADMIN.ROLE.GET_ALL).pipe(
      map(response => {
        if (response.success && response.data) {
          this.roles.set(response.data);
          return response.data;
        }
        throw new Error(response.message || 'Lỗi khi lấy danh sách vai trò');
      }),
      // Complete loading in consumer or here, but better to handle it there if needed.
      // We will let the consumer toggle loading.off, or do it here on finalize
    );
  }

  /**
   * Create a new role
   */
  createRole(data: ICreateRoleRequest): Observable<IRole> {
    // Note: The API in RoleController maps to POST /api/Role
    // But our API_ENDPOINTS.ADMIN.ROLE.CREATE uses /api/admin/roles
    // Since RoleController mapping might be /api/role, let's use the explicit string if needed.
    // Wait, the user said they are okay with /api/Role or /api/admin/roles.
    // I will use API_ENDPOINTS.ADMIN.ROLE.CREATE which maps to /api/admin/roles as per endpoints file.
    // If it fails, we will adjust it.
    return this.http.post<ApiResponse<IRole>>('/api/Role', data).pipe(
      map(response => {
        if (response.success && response.data) {
          // Add the new role to the signals state
          this.roles.update(r => [...r, response.data!]);
          return response.data;
        }
        throw new Error(response.message || 'Lỗi khi tạo vai trò');
      })
    );
  }

  /**
   * Get all available permissions grouped by module
   */
  fetchAllPermissions(): Observable<IPermissionGroup[]> {
    return this.http.get<ApiResponse<{ [key: string]: IPermissionItem[] }>>(API_ENDPOINTS.ADMIN.ROLE.GET_ALL_PERMISSIONS).pipe(
      map(response => {
        if (response.success && response.data) {
          const data = response.data;
          // Map dictionary to array of IPermissionGroup
          const mapped: IPermissionGroup[] = Object.keys(data).map(key => ({
            groupName: key,
            permissions: data[key]
          }));
          this.allPermissionsGrouped.set(mapped);
          return mapped;
        }
        throw new Error(response.message || 'Lỗi khi lấy danh sách quyền');
      })
    );
  }

  /**
   * Get permission keys assigned to a specific role
   */
  getRolePermissions(roleId: number): Observable<string[]> {
    return this.http.get<ApiResponse<string[]>>(API_ENDPOINTS.ADMIN.ROLE.GET_PERMISSIONS(roleId)).pipe(
      map(response => {
        if (response.success && response.data) {
          return response.data;
        }
        throw new Error(response.message || 'Lỗi khi lấy quyền của vai trò');
      })
    );
  }

  /**
   * Update permissions for a specific role
   */
  updateRolePermissions(roleId: number, permissionKeys: string[]): Observable<boolean> {
    return this.http.put<ApiResponse<boolean>>(API_ENDPOINTS.ADMIN.ROLE.UPDATE_PERMISSIONS(roleId), { permissionKeys }).pipe(
      map(response => {
        if (response.success) {
          return true;
        }
        throw new Error(response.message || 'Lỗi khi cập nhật quyền');
      })
    );
  }
}
