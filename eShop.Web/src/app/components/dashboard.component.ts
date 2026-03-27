import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container mt-5">
      <div class="row">
        <div class="col-md-12">
          <h1>Welcome to eShop</h1>
          <p class="lead">Explore our products or manage your account</p>
          <a routerLink="/products" class="btn btn-primary btn-lg">View Products</a>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent { }
