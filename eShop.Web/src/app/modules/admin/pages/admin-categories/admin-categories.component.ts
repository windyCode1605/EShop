import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminCategoryService } from '../../services/admin-category.service';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';
import { CategoryFormComponent } from './category-form/category-form.component';
import { IAdminCategory, ICategoryCreateDto } from '../../models/admin-category.model';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmDialogComponent, CategoryFormComponent, ImageUrlPipe],
  styleUrl: './admin-categories.component.scss',
  templateUrl: './admin-categories.component.html'
})
export class AdminCategoriesComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  categories  = signal<IAdminCategory[]>([]);
  loading     = signal(false);
  searchKeyword = '';

  formVisible      = signal(false);
  selectedCategory = signal<IAdminCategory | null>(null);
  confirmVisible   = signal(false);
  deleteTarget     = signal<IAdminCategory | null>(null);

  toasts = signal<{ id: number; type: 'success' | 'error'; message: string }[]>([]);
  private toastCounter = 0;

  // Computed
  filteredCategories = computed(() => {
    const kw = this.searchKeyword.toLowerCase();
    if (!kw) return this.categories();
    return this.categories().filter(c =>
      c.categoryName.toLowerCase().includes(kw) ||
      c.slug.toLowerCase().includes(kw)
    );
  });

  activeCount      = computed(() => this.categories().filter(c => c.isActive).length);
  inactiveCount    = computed(() => this.categories().filter(c => !c.isActive).length);
  rootCategories   = computed(() => this.categories().filter(c => !c.parentId));
  totalProductCount = computed(() => this.categories().reduce((s, c) => s + (c.productCount ?? 0), 0));

  readonly skeletons = Array(6).fill(0);

  constructor(private categoryService: AdminCategoryService) { }

  ngOnInit(): void {
    this.loadCategories();
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.categoryService.getCategories(1, 100)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          this.categories.set(res.items);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
          this.showToast('error', 'Không thể tải danh sách danh mục');
        }
      });
  }

  onSearch(kw: string): void {
    this.searchKeyword = kw;
    this.searchSubject.next(kw);
  }

  toggleActive(cat: IAdminCategory): void {
    cat.isActive = !cat.isActive;
    this.categoryService.updateCategory(cat.categoryId, cat).subscribe({
      next: () => this.showToast('success', `Đã \${cat.isActive ? 'hiện' : 'ẩn'} danh mục`),
      error: () => {
        cat.isActive = !cat.isActive; // rollback
        this.showToast('error', 'Lỗi khi cập nhật trạng thái');
      }
    });
  }

  openCreateForm(): void {
    this.selectedCategory.set(null);
    this.formVisible.set(true);
  }

  openEditForm(cat: IAdminCategory): void {
    this.selectedCategory.set(cat);
    this.formVisible.set(true);
  }

  closeForm(): void {
    this.formVisible.set(false);
    this.selectedCategory.set(null);
  }

  onFormSaved(isNew: boolean): void {
    this.loadCategories();
    this.showToast('success', isNew ? 'Thêm mới danh mục thành công' : 'Cập nhật danh mục thành công');
  }

  confirmDelete(cat: IAdminCategory): void {
    this.deleteTarget.set(cat);
    this.confirmVisible.set(true);
  }

  cancelDelete(): void {
    this.confirmVisible.set(false);
    this.deleteTarget.set(null);
  }

  onDeleteConfirmed(): void {
    const target = this.deleteTarget();
    if (!target) return;

    this.categoryService.deleteCategory(target.categoryId).subscribe({
      next: () => {
        this.categories.update(arr => arr.filter(c => c.categoryId !== target.categoryId));
        this.confirmVisible.set(false);
        this.deleteTarget.set(null);
        this.showToast('success', 'Đã xóa danh mục');
      },
      error: () => {
        this.confirmVisible.set(false);
        this.showToast('error', 'Lỗi khi xóa danh mục');
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