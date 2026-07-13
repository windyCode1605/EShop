import {
  Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IAdminCategory, ICategoryCreateDto } from '../../../models/admin-category.model';
import { AdminCategoryService } from '../../../services/admin-category.service';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

  constructor(private categoryService: AdminCategoryService) {}

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
          this.saved.emit(false);
          this.onClose();
        },
        error: () => this.saving.set(false)
      });
    } else {
      this.categoryService.createCategory(this.form).subscribe({
        next: () => {
          this.saving.set(false);
          this.saved.emit(true);
          this.onClose();
        },
        error: () => this.saving.set(false)
      });
    }
  }
}