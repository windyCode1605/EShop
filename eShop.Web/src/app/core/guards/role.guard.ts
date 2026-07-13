import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../service-proxies/token.service';
import { PermissionStore } from '../stores/permission.store';

/**
 * roleGuard — kiểm tra user có role phù hợp không.
 *
 * Cách dùng trong routes:
 *   canActivate: [authGuard, roleGuard],
 *   data: { roles: ['Admin', 'SuperAdmin'] }
 *
 * Guard check theo 2 tầng:
 *   1. PermissionStore.roles (dynamic roles từ /api/me/permissions)
 *   2. TokenService.isAdmin() (fallback từ UserType trong JWT)
 */
export const roleGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const permissionStore = inject(PermissionStore);
  const router = inject(Router);

  const expectedRoles = route.data?.['roles'] as string[] | undefined;

  // Nếu route không yêu cầu role cụ thể → cho qua
  if (!expectedRoles || expectedRoles.length === 0) return true;

  // Lấy roles từ PermissionStore (đã fetch từ API sau login)
  const storeRoles = permissionStore.roles();

  // Check: có bất kỳ role nào trong expectedRoles không?
  const hasRole = expectedRoles.some(expected =>
    storeRoles.some(r => r.toLowerCase() === expected.toLowerCase())
  );

  if (hasRole) return true;

  // Fallback: Nếu backend chưa trả về role rõ ràng, nhưng JWT Token xác nhận đây là Admin
  // thì vẫn cho phép vào các trang quản trị (đòi hỏi role Admin/SuperAdmin)
  const isAdmin = tokenService.isAdmin();
  if (isAdmin && expectedRoles.some(r => r.toLowerCase() === 'admin' || r.toLowerCase() === 'super_admin')) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};
