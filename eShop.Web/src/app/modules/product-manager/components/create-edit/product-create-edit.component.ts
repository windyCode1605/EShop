import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '../../services/product.service';
import { ProductCreateUpdateDto } from '../../models/product.model';

/**
 * Product Create/Edit Component (Smart Component)
 * Handles product creation and editing form
 */
@Component({
  selector: 'app-product-create-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './product-create-edit.component.html',
  styleUrls: ['./product-create-edit.component.scss']
})
export class ProductCreateEditComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  loading: boolean = false;
  submitting: boolean = false;
  error: string | null = null;
  isEditMode: boolean = false;
  categories: any[] = []; // Load from API
  productId: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.checkEditMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Initialize form
   */
  private initializeForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(255)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(2000)]],
      price: [0, [Validators.required, Validators.min(0)]],
      categoryId: ['', Validators.required],
      stock: [0, [Validators.required, Validators.min(0)]],
      sku: ['', [Validators.required, Validators.minLength(3)]],
      image: [''],
      isActive: [true]
    });
  }

  /**
   * Check if editing existing product
   */
  private checkEditMode(): void {
    this.route.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        this.productId = params.get('id');
        if (this.productId) {
          this.isEditMode = true;
          this.loadProductForEdit(this.productId);
        }
      });
  }

  /**
   * Load product for editing
   */
  private loadProductForEdit(id: string): void {
    this.loading = true;
    this.productService
      .getProductById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (product) => {
          this.form.patchValue(product);
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load product for editing.';
          this.loading = false;
          console.error('Error loading product:', err);
        }
      });
  }

  /**
   * Submit form
   */
  onSubmit(): void {
    if (!this.form.valid) {
      this.markFormAsUntouched();
      return;
    }

    this.submitting = true;
    this.error = null;

    const formData: ProductCreateUpdateDto = this.form.value;

    if (this.isEditMode && this.productId) {
      this.updateProduct(this.productId, formData);
    } else {
      this.createProduct(formData);
    }
  }

  /**
   * Create new product
   */
  private createProduct(data: ProductCreateUpdateDto): void {
    this.productService
      .createProduct(data)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (product) => {
          this.submitting = false;
          this.router.navigate(['/product-manager/detail', product.id]);
        },
        error: (err) => {
          this.submitting = false;
          this.error = 'Failed to create product.';
          console.error('Error creating product:', err);
        }
      });
  }

  /**
   * Update existing product
   */
  private updateProduct(id: string, data: ProductCreateUpdateDto): void {
    this.productService
      .updateProduct(id, data)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (product) => {
          this.submitting = false;
          this.router.navigate(['/product-manager/detail', product.id]);
        },
        error: (err) => {
          this.submitting = false;
          this.error = 'Failed to update product.';
          console.error('Error updating product:', err);
        }
      });
  }

  /**
   * Navigate back
   */
  goBack(): void {
    this.router.navigate(['/product-manager']);
  }

  /**
   * Mark form fields as touched
   */
  private markFormAsUntouched(): void {
    Object.keys(this.form.controls).forEach(key => {
      this.form.get(key)?.markAsTouched();
    });
  }

  /**
   * Get form control
   */
  getControl(name: string) {
    return this.form.get(name);
  }

  /**
   * Check if field has error
   */
  hasError(fieldName: string, errorType: string): boolean {
    const control = this.form.get(fieldName);
    return !!(control && control.hasError(errorType) && (control.dirty || control.touched));
  }
}
