import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ProductDto } from '../models/index';

@Injectable({ providedIn: 'root' })
export class ProductService {
  getProducts(): Observable<ProductDto[]> {
    return of([]);
  }
}
