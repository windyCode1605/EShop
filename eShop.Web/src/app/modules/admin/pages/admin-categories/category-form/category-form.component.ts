import {
  Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IAdminCategory, ICategoryCreateDto } from '../../../models/admin-category.model';
import { AdminCategoryService } from '../../../services/admin-category.service';
import { ToastService } from '../../../../../core/services/toast.service';
import { ImageUrlPipe } from '../../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ImageUrlPipe],
  styleUrl: './category-form.component.scss',
  templateUrl: './category-form.component.html'
})
export class CategoryFormComponent {
  @Input() visible = false;
  
  _category: IAdminCategory | null = null;
  @Input() set category(val: IAdminCategory | null) {
    this._category = val;
    if (val) {
      this.isEdit = true;
      this.form = { ...val };
    } else {
      this.isEdit = false;
      this.form = {
        categoryName: '',
        slug: '',
        parentId: null,
        description: '',
        imageUrl: '',
        sortOrder: 0,
        isActive: true
      };
    }
    this.submitted = false;
  }
  get category() { return this._category; }
  
  @Input() allCategories: IAdminCategory[] = [];

  @Output() closed = new EventEmitter<void>();
  @Output() saved = new EventEmitter<boolean>();

  isEdit = false;
  saving = signal(false);
  submitted = false;
  form: any = {};

  constructor(
    private categoryService: AdminCategoryService,
    private toastService: ToastService
  ) {}

  onNameChange(name: string) {
    if (!this.isEdit && name) {
      this.form.slug = this.slugify(name);
    }
  }

  generateSlug() {
    if (this.form.categoryName) {
      this.form.slug = this.slugify(this.form.categoryName);
    }
  }

  private slugify(text: string): string {
    return text.toString().toLowerCase()
      .replace(/đ/g, 'd').replace(/Đ/g, 'D') // Vietnamese support
      .normalize('NFD').replace(/[\u0300-\u036f]/g, '') // Remove accents
      .replace(/\s+/g, '-')           // Replace spaces with -
      .replace(/[^\w\-]+/g, '')       // Remove all non-word chars
      .replace(/\-\-+/g, '-')         // Replace multiple - with single -
      .replace(/^-+/, '')             // Trim - from start of text
      .replace(/-+$/, '');            // Trim - from end of text
  }

  onImageError() {
    this.form.imageUrl = '';
  }

  onClose(): void {
    if (!this.saving()) this.closed.emit();
  }

  onSubmit(): void {
    this.submitted = true;
    if (!this.form.categoryName || !this.form.slug) return;
    this.saving.set(true);

    if (this.isEdit) {
      this.categoryService.updateCategory(this.form.categoryId, this.form).subscribe({
        next: () => {
          this.saving.set(false);
          this.toastService.success('Cập nhật danh mục thành công');
          this.saved.emit(false);
          this.onClose();
        },
        error: (err: any) => {
          this.saving.set(false);
          this.toastService.error(err?.error?.message || 'Có lỗi xảy ra khi cập nhật danh mục');
        }
      });
    } else {
      this.categoryService.createCategory(this.form).subscribe({
        next: () => {
          this.saving.set(false);
          this.toastService.success('Tạo danh mục thành công');
          this.saved.emit(true);
          this.onClose();
        },
        error: (err: any) => {
          this.saving.set(false);
          this.toastService.error(err?.error?.message || 'Có lỗi xảy ra khi tạo danh mục');
        }
      });
    }
  }
}