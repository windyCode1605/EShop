import {
  Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, signal, computed, ViewChild, ElementRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import {
  IAdminProduct,
  IProductCreateDto,
  IProductImage,
  IProductAttribute,
  IAdminProductVariant,
  IProductVariantAttribute,
  AttributeType
} from '../../../models/admin-product.model';
import { IAdminCategory } from '../../../models/admin-category.model';
import { AdminProductService } from '../../../services/admin-product.service';

type FormTab = 'info' | 'images' | 'attributes' | 'variants';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  styleUrl: './product-form.component.scss',
  templateUrl: './product-form.component.html'
})
export class ProductFormComponent {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  
  @Input() visible = false;
  @Input() set product(val: IAdminProduct | null) {
    if (val) {
      this.isEdit = true;
      this.form = { ...val };
      this.images.set(val.variants?.flatMap(v => v.imageUrl ? [{ imageUrl: v.imageUrl, isPrimary: false }] : []) as any || []);
      this.attributes.set([]);
      this.variants.set(val.variants || []);
    } else {
      this.isEdit = false;
      this.form = {
        name: '', sku: '', price: 0, stock: 0, categoryId: null,
        description: '', aI_Description: '', isActive: true, variants: []
      };
      this.images.set([]);
      this.attributes.set([]);
      this.variants.set([]);
    }
    this.submitted = false;
    this.activeTab = 'info';
  }
  @Input() categories: IAdminCategory[] = [];

  @Output() closed = new EventEmitter<void>();
  @Output() saved = new EventEmitter<boolean>();

  isEdit = false;
  saving = signal(false);
  submitted = false;
  form: any = {};
  
  activeTab: FormTab = 'info';
  
  // Images
  images = signal<any[]>([]);
  newImageUrl = '';
  newImageAlt = '';
  
  // Attributes
  attributes = signal<any[]>([]);
  showAttrForm = false;
  newAttr = { name: '', type: 'Text' as AttributeType };
  newValueLabel: string[] = [];
  newValueRaw: string[] = [];
  
  // Variants
  variants = signal<any[]>([]);
  expandedVariants = new Set<number>();
  showVariantForm = false;
  newVariant: any = { sku: '', priceAdjustment: 0, stockQuantity: 0 };
  newVariantAttrValues: string[] = [];
  newVariantAttrCustom: string[] = [];

  constructor(private productService: AdminProductService) {}

  setTab(tab: FormTab) { this.activeTab = tab; }

  onClose(): void {
    if (!this.saving()) this.closed.emit();
  }

  onSubmit(): void {
    this.submitted = true;
    if (!this.form.name || !this.form.categoryId) {
      this.activeTab = 'info';
      return;
    }
    this.saving.set(true);

    const payload = { ...this.form, variants: this.variants() };

    if (this.isEdit) {
      this.productService.updateProduct(this.form.id, payload).subscribe({
        next: () => { this.saving.set(false); this.saved.emit(false); this.onClose(); },
        error: () => this.saving.set(false)
      });
    } else {
      this.productService.createProduct(payload).subscribe({
        next: () => { this.saving.set(false); this.saved.emit(true); this.onClose(); },
        error: () => this.saving.set(false)
      });
    }
  }

  // --- Images ---
  setPrimaryImage(i: number) {
    this.images.update(imgs => imgs.map((img, idx) => ({ ...img, isPrimary: idx === i })));
  }
  removeImage(i: number) { this.images.update(imgs => imgs.filter((_, idx) => idx !== i)); }
  addImageByUrl() {
    if (this.newImageUrl) {
      this.images.update(imgs => [...imgs, { imageUrl: this.newImageUrl, altText: this.newImageAlt, isPrimary: imgs.length === 0 }]);
      this.newImageUrl = '';
      this.newImageAlt = '';
    }
  }
  triggerFileUpload() { if(this.fileInput) this.fileInput.nativeElement.click(); }
  onFileChange(e: any) {}

  // --- Attributes ---
  getAttrTypeLabel(t: string) { return t; }
  removeAttribute(ai: number) { this.attributes.update(arr => arr.filter((_, i) => i !== ai)); }
  removeAttributeValue(ai: number, vi: number) {
    this.attributes.update(arr => {
      const copy = [...arr];
      copy[ai].values = copy[ai].values.filter((_: any, i: number) => i !== vi);
      return copy;
    });
  }
  addAttributeValue(ai: number) {
    const lbl = this.newValueLabel[ai];
    const raw = this.newValueRaw[ai] || lbl;
    if (lbl) {
      this.attributes.update(arr => {
        const copy = [...arr];
        copy[ai].values.push({ label: lbl, value: raw });
        return copy;
      });
      this.newValueLabel[ai] = '';
      this.newValueRaw[ai] = '';
    }
  }
  addAttribute() {
    if (this.newAttr.name) {
      this.attributes.update(arr => [...arr, { name: this.newAttr.name, type: this.newAttr.type, values: [] }]);
      this.newAttr = { name: '', type: 'Text' };
      this.showAttrForm = false;
    }
  }

  // --- Variants ---
  toggleVariantExpand(vi: number) {
    if (this.expandedVariants.has(vi)) this.expandedVariants.delete(vi);
    else this.expandedVariants.add(vi);
  }
  removeVariant(vi: number) { this.variants.update(arr => arr.filter((_, i) => i !== vi)); }
  setDefaultVariant(vi: number) {
    this.variants.update(arr => arr.map((v, i) => ({ ...v, isDefault: i === vi })));
  }
  addVariant() {
    if (this.newVariant.sku) {
      if (this.isEdit && this.form.id) {
        // Luồng 2: Gọi API trực tiếp để tạo Variant cho Product đã tồn tại
        this.saving.set(true);
        const dto = {
          productId: this.form.id,
          sku: this.newVariant.sku,
          priceAdjustment: this.newVariant.priceAdjustment,
          stockQuantity: this.newVariant.stockQuantity,
          isDefault: this.variants().length === 0,
          isActive: true,
          attributes: []
        };
        
        this.productService.createProductVariant(dto).subscribe({
          next: (res) => {
            this.variants.update(arr => [...arr, res]);
            this.newVariant = { sku: '', priceAdjustment: 0, stockQuantity: 0 };
            this.showVariantForm = false;
            this.saving.set(false);
            // Optionally emit saved event if you want the parent to reload
            this.saved.emit(false); 
          },
          error: () => {
            this.saving.set(false);
          }
        });
      } else {
        // Luồng 1: Chỉ lưu vào mảng tạm thời, gửi đi khi nhấn Save Product
        this.variants.update(arr => [...arr, { ...this.newVariant, isActive: true, isDefault: arr.length === 0, attributes: [] }]);
        this.newVariant = { sku: '', priceAdjustment: 0, stockQuantity: 0 };
        this.showVariantForm = false;
      }
    }
  }
}