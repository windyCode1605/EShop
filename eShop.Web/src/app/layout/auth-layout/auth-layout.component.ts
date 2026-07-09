import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FooterComponent } from '../components/footer.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, FooterComponent],
  template: `<router-outlet></router-outlet>`,
  styles: []
})
export class AuthLayoutComponent { }
