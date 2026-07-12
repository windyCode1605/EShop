import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionStore } from '../stores/permission.store';

/**
 * permissionGuard — kiểm tra user có permission key cụ thể không.
 *
 * Cách dùng trong routes:
 *   canActivate: [authGuard, permissionGuard],
 *   data: { permissions: ['Products.Create', 'Products.View'] }
 *
 * User chỉ cần CÓ 1 trong các permission → được vào.
 */
export const permissionGuard: CanActivateFn = (route, state) => {
  const permissionStore = inject(PermissionStore);
  const router = inject(Router);

  const requiredPermissions = route.data?.['permissions'] as string[] | undefined;

  // Nếu route không yêu cầu permission → cho qua
  if (!requiredPermissions || requiredPermissions.length === 0) return true;

  const hasAny = requiredPermissions.some(p => permissionStore.hasPermission(p));

  if (hasAny) return true;

  return router.createUrlTree(['/dashboard']);
};
