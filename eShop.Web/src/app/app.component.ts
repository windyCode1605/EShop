import { Component } from '@angular/core';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { NavbarComponent } from './layout/components/navbar.component';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, NavbarComponent, CommonModule, ToastModule],
  template: `
    <p-toast></p-toast>
    <app-navbar *ngIf="!hideNavbar"></app-navbar>
    <router-outlet></router-outlet>
  `,
  styles: [],
})
export class AppComponent {
  title = 'eShop.Web';
  hideNavbar = false;

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.hideNavbar = event.url.includes('/account/') || event.url === '/login' || event.url.includes('/admin');
    });
  }
}
