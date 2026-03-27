import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ProductDto, CreateProductDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) { }

  getProducts(): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(this.apiUrl);
  }

  getProduct(id: number): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.apiUrl, product);
  }

  updateProduct(id: number, product: CreateProductDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
