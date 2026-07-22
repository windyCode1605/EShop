import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminProductService, AdminProductListResponse } from '../../services/admin-product.service';
import { AdminCategoryService } from '../../services/admin-category.service';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';
import { ProductFormComponent } from './product-form/product-form.component';
import {
  IAdminProduct,
  IAdminProductFilter,
  IProductCreateDto,
  IProductImage,
  IProductAttribute,
  IAdminProductVariant
} from '../../models/admin-product.model';
import { IAdminCategory } from '../../models/admin-category.model';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, ProductFormComponent, ConfirmDialogComponent],
  styleUrl: './admin-products.component.scss',
  templateUrl: './admin-products.component.html'
})
export class AdminProductsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  products = signal<IAdminProduct[]>([]);
  totalCount = signal(0);
  loading = signal(false);

  // Pagination & Filter Signals
  currentPage = signal(1);
  pageSize = signal(10);
  filter = { categoryId: '', isActive: '' };
  searchKeyword = '';
  sortValue = 'createdAt_desc'; // Default sort

  // Computed signals for Stats
  activeCount = computed(() => this.products().filter(p => p.isActive).length);
  inactiveCount = computed(() => this.products().filter(p => !p.isActive).length);
  lowStockCount = computed(() => this.products().filter(p => p.stock < 10).length);
  totalVariants = computed(() => this.products().reduce((sum, p) => sum + (p.variants?.length || 0), 0));
  
  // Computed Pagination
  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()) || 1);
  paginationFrom = computed(() => (this.currentPage() - 1) * this.pageSize() + 1);
  paginationTo = computed(() => Math.min(this.currentPage() * this.pageSize(), this.totalCount()));
  paginationRange = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const range: number[] = [];
    for (let i = 1; i <= total; i++) {
      if (i === 1 || i === total || (i >= current - 1 && i <= current + 1)) {
        range.push(i);
      } else if (range[range.length - 1] !== -1) {
        range.push(-1);
      }
    }
    return range;
  });

  categories = signal<IAdminCategory[]>([]);
  
  formVisible = signal(false);
  selectedProduct = signal<IAdminProduct | null>(null);
  
  confirmVisible = signal(false);
  deleteTarget = signal<IAdminProduct | null>(null);

  toasts = signal<{ id: number; type: 'success' | 'error'; message: string }[]>([]);
  private toastCounter = 0;

  readonly skeletons = Array(5).fill(0);

  constructor(
    private productService: AdminProductService,
    private categoryService: AdminCategoryService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.currentPage.set(1);
      this.loadProducts();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe(res => {
      this.categories.set(res.items);
    });
  }

  loadProducts(): void {
    this.loading.set(true);
    let sortBy = 'CreatedAt';
    let sortDir = 'desc';

    if (this.sortValue) {
      const parts = this.sortValue.split('_');
      if (parts.length === 2) {
        sortBy = parts[0];
        sortDir = parts[1];
      }
    }

    this.productService.getProducts({
      pageIndex: this.currentPage(),
      pageSize: this.pageSize(),
      keyword: this.searchKeyword,
      categoryId: this.filter.categoryId ? Number(this.filter.categoryId) : undefined,
      isActive: this.filter.isActive ? this.filter.isActive === 'true' : undefined,
      sortBy: sortBy,
      sortOrder: sortDir as any
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.products.set(res.items);
          this.totalCount.set(res.totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
          this.showToast('error', 'Lỗi khi tải danh sách sản phẩm');
        }
      });
  }

  applyFilter(): void {
    this.currentPage.set(1);
    this.loadProducts();
  }

  onSearch(kw: string): void {
    this.searchKeyword = kw;
    this.searchSubject.next(kw);
  }

  onSortChange(val: string): void {
    this.sortValue = val;
    this.currentPage.set(1);
    this.loadProducts();
  }

  sort(field: string): void {
    if (this.sortValue.startsWith(field)) {
      this.sortValue = this.sortValue.endsWith('asc') ? field + '_desc' : field + '_asc';
    } else {
      this.sortValue = field + '_asc';
    }
    this.currentPage.set(1);
    this.loadProducts();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadProducts();
    }
  }

  getProductImage(p: IAdminProduct): string {
    return p.variants?.[0]?.imageUrls?.[0] || 'assets/placeholder.png';
  }

  openCreateForm(): void {
    this.selectedProduct.set(null);
    this.formVisible.set(true);
  }

  openEditForm(p: IAdminProduct): void {
    this.selectedProduct.set(p);
    this.formVisible.set(true);
  }

  closeForm(): void {
    this.formVisible.set(false);
    this.selectedProduct.set(null);
  }

  onFormSaved(success: boolean): void {
    this.loadProducts();
    this.showToast('success', success ? 'Tạo sản phẩm thành công' : 'Cập nhật sản phẩm thành công');
  }

  confirmDelete(p: IAdminProduct): void {
    this.deleteTarget.set(p);
    this.confirmVisible.set(true);
  }

  cancelDelete(): void {
    this.confirmVisible.set(false);
    this.deleteTarget.set(null);
  }

  onDeleteConfirmed(): void {
    const target = this.deleteTarget();
    if (!target) return;

    this.productService.deleteProduct(target.id).subscribe({
      next: () => {
        this.products.update(arr => arr.filter(x => x.id !== target.id));
        this.totalCount.update(c => c - 1);
        this.confirmVisible.set(false);
        this.deleteTarget.set(null);
        this.showToast('success', 'Đã xóa sản phẩm');
      },
      error: () => {
        this.confirmVisible.set(false);
        this.showToast('error', 'Lỗi khi xóa sản phẩm');
      }
    });
  }

  private showToast(type: 'success' | 'error', message: string): void {
    const id = ++this.toastCounter;
    this.toasts.update(t => [...t, { id, type, message }]);
    setTimeout(() => {
      this.toasts.update(t => t.filter(x => x.id !== id));
    }, 3000);
  }
}