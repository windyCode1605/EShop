import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.scss']
})
export class ProductPageComponent implements OnInit {
  categories = [
    { id: 1, name: 'Laptops & Computers', count: 124 },
    { id: 2, name: 'Smartphones & Tablets', count: 86 },
    { id: 3, name: 'Audio & Headphones', count: 45 },
    { id: 4, name: 'Gaming Consoles', count: 32 },
    { id: 5, name: 'Wearable Tech', count: 57 },
    { id: 6, name: 'Cameras & Photography', count: 21 },
  ];

  products = [
    {
      id: 1,
      name: 'MacBook Pro 16" M3 Max',
      price: 3499,
      originalPrice: 3699,
      rating: 4.9,
      reviews: 128,
      imageUrl: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&q=80&w=800',
      badge: 'Bán chạy',
      isNew: false
    },
    {
      id: 2,
      name: 'Sony WH-1000XM5 Wireless Headphones',
      price: 398,
      originalPrice: null,
      rating: 4.8,
      reviews: 256,
      imageUrl: 'https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?auto=format&fit=crop&q=80&w=800',
      badge: '',
      isNew: true
    },
    {
      id: 3,
      name: 'iPhone 15 Pro Max 256GB',
      price: 1199,
      originalPrice: 1299,
      rating: 4.9,
      reviews: 512,
      imageUrl: 'https://images.unsplash.com/photo-1696446701796-da61225697cc?auto=format&fit=crop&q=80&w=800',
      badge: 'Giảm giá',
      isNew: false
    },
    {
      id: 4,
      name: 'PlayStation 5 Console',
      price: 499,
      originalPrice: null,
      rating: 4.9,
      reviews: 890,
      imageUrl: 'https://images.unsplash.com/photo-1606813907291-d86efa9b94db?auto=format&fit=crop&q=80&w=800',
      badge: '',
      isNew: false
    },
    {
      id: 5,
      name: 'Apple Watch Series 9',
      price: 399,
      originalPrice: 429,
      rating: 4.7,
      reviews: 184,
      imageUrl: 'https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?auto=format&fit=crop&q=80&w=800',
      badge: '',
      isNew: true
    },
    {
      id: 6,
      name: 'iPad Pro 12.9" M2',
      price: 1099,
      originalPrice: null,
      rating: 4.8,
      reviews: 320,
      imageUrl: 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?auto=format&fit=crop&q=80&w=800',
      badge: '',
      isNew: false
    }
  ];

  constructor() {}

  ngOnInit(): void {}
}
