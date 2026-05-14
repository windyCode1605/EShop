import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { IProduct, ProductModel, ProductResponseDto, ProductCreateUpdateDto } from '../models/product.model';
import { ProductFilterModel } from '../models/product-filter.model';
import { ProductHttpService } from './product-http.service';

/**
 * Product Service
 * Manages product business logic and state
 */
@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private productsSubject = new BehaviorSubject<IProduct[]>([]);
  private selectedProductSubject = new BehaviorSubject<IProduct | null>(null);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private filterSubject = new BehaviorSubject<ProductFilterModel>(new ProductFilterModel());

  public products$ = this.productsSubject.asObservable();
  public selectedProduct$ = this.selectedProductSubject.asObservable();
  public loading$ = this.loadingSubject.asObservable();
  public filter$ = this.filterSubject.asObservable();

  constructor(private productHttpService: ProductHttpService) {}

  /**
   * Get all products with filter
   */
  getProducts(filter: ProductFilterModel): Observable<ProductResponseDto> {
    this.loadingSubject.next(true);
    this.filterSubject.next(filter);
    return new Observable(observer => {
      this.productHttpService.getProducts(filter).subscribe({
        next: (response) => {
          this.productsSubject.next(response.items);
          this.loadingSubject.next(false);
          observer.next(response);
          observer.complete();
        },
        error: (err) => {
          this.loadingSubject.next(false);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Get product by ID
   */
  getProductById(id: string | number): Observable<IProduct> {
    this.loadingSubject.next(true);
    return new Observable(observer => {
      this.productHttpService.getProductById(id).subscribe({
        next: (response) => {
          this.selectedProductSubject.next(response);
          this.loadingSubject.next(false);
          observer.next(response);
          observer.complete();
        },
        error: (err) => {
          this.loadingSubject.next(false);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Create new product
   */
  createProduct(product: ProductCreateUpdateDto): Observable<IProduct> {
    this.loadingSubject.next(true);
    return new Observable(observer => {
      this.productHttpService.createProduct(product).subscribe({
        next: (response) => {
          const currentProducts = this.productsSubject.value;
          this.productsSubject.next([response, ...currentProducts]);
          this.loadingSubject.next(false);
          observer.next(response);
          observer.complete();
        },
        error: (err) => {
          this.loadingSubject.next(false);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Update existing product
   */
  updateProduct(id: string | number, product: ProductCreateUpdateDto): Observable<IProduct> {
    this.loadingSubject.next(true);
    return new Observable(observer => {
      this.productHttpService.updateProduct(id, product).subscribe({
        next: (response) => {
          const currentProducts = this.productsSubject.value;
          const index = currentProducts.findIndex(p => p.id === id);
          if (index > -1) {
            currentProducts[index] = response;
            this.productsSubject.next([...currentProducts]);
          }
          this.selectedProductSubject.next(response);
          this.loadingSubject.next(false);
          observer.next(response);
          observer.complete();
        },
        error: (err) => {
          this.loadingSubject.next(false);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Delete product
   */
  deleteProduct(id: string | number): Observable<void> {
    this.loadingSubject.next(true);
    return new Observable(observer => {
      this.productHttpService.deleteProduct(id).subscribe({
        next: () => {
          const currentProducts = this.productsSubject.value;
          this.productsSubject.next(currentProducts.filter(p => p.id !== id));
          this.loadingSubject.next(false);
          observer.next();
          observer.complete();
        },
        error: (err) => {
          this.loadingSubject.next(false);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Set selected product
   */
  setSelectedProduct(product: IProduct | null): void {
    this.selectedProductSubject.next(product);
  }

  /**
   * Set loading state
   */
  setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  /**
   * Get current filter
   */
  getCurrentFilter(): ProductFilterModel {
    return this.filterSubject.value;
  }
}
