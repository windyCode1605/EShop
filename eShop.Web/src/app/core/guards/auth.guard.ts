import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { TokenService } from '../service-proxies/token.service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.getToken()) {
    return true;
  }

  // Chưa đăng nhập thì chuyển hướng về trang login
  return router.parseUrl('/account/login');
};
