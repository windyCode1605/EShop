import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../service-proxies/token.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  const expectedRoles = route.data?.['route'] as Array<string>;
  const userRole = tokenService.getUserRole();
  if (expectedRoles && expectedRoles.includes(userRole))
    return true;

  return router.createUrlTree(['/dashboard']);
};
