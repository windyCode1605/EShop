import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AddressService, SaveAddressRequestDto } from '../../../../core/services/address.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-address-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <!-- Backdrop -->
    <div *ngIf="isOpen" class="fixed inset-0 z-50 flex items-center justify-center">
      <div class="absolute inset-0 bg-zinc-900/40 backdrop-blur-sm transition-opacity" (click)="close()"></div>
      
      <!-- Modal Content -->
      <div class="relative w-full max-w-lg bg-white rounded-[24px] shadow-2xl overflow-hidden m-4 flex flex-col max-h-[90dvh]">
        
        <div class="px-6 py-5 border-b border-zinc-100 flex justify-between items-center bg-white sticky top-0 z-10">
          <h3 class="text-lg font-medium text-zinc-900">Thêm địa chỉ mới</h3>
          <button type="button" (click)="close()" class="p-2 text-zinc-400 hover:text-zinc-900 transition-colors rounded-full hover:bg-zinc-50">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>

        <div class="p-6 overflow-y-auto">
          <form [formGroup]="addressForm" (ngSubmit)="onSave()" class="grid gap-5">
            
            <div class="grid grid-cols-2 gap-5">
              <div class="form-group col-span-2 sm:col-span-1">
                <label class="block text-sm font-medium text-zinc-700 mb-2">Tên người nhận</label>
                <input type="text" formControlName="receiverName" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors" placeholder="Vd: Nguyễn Văn A" />
              </div>
              <div class="form-group col-span-2 sm:col-span-1">
                <label class="block text-sm font-medium text-zinc-700 mb-2">Số điện thoại</label>
                <input type="text" formControlName="receiverPhone" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors" placeholder="Vd: 0912345678" />
              </div>
            </div>

            <div class="form-group">
              <label class="block text-sm font-medium text-zinc-700 mb-2">Tỉnh / Thành phố</label>
              <input type="text" formControlName="province" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors" placeholder="Vd: Hà Nội" />
            </div>

            <div class="form-group">
              <label class="block text-sm font-medium text-zinc-700 mb-2">Quận / Huyện</label>
              <input type="text" formControlName="city" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors" placeholder="Vd: Cầu Giấy" />
            </div>

            <div class="form-group">
              <label class="block text-sm font-medium text-zinc-700 mb-2">Địa chỉ cụ thể (Số nhà, tên đường)</label>
              <textarea formControlName="street" rows="3" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors resize-none" placeholder="Vd: Số 1, Ngõ 1, Đường 1"></textarea>
            </div>

            <div class="flex items-center gap-3 mt-2">
              <input type="checkbox" formControlName="isDefault" id="isDefault" class="w-4 h-4 text-zinc-900 border-zinc-300 rounded focus:ring-zinc-900" />
              <label for="isDefault" class="text-sm font-medium text-zinc-700 select-none cursor-pointer">Đặt làm địa chỉ mặc định</label>
            </div>

          </form>
        </div>

        <div class="px-6 py-5 border-t border-zinc-100 bg-zinc-50 flex justify-end gap-3 rounded-b-[24px]">
          <button type="button" (click)="close()" class="px-6 py-2.5 text-sm font-medium text-zinc-600 hover:text-zinc-900 hover:bg-zinc-100 rounded-xl transition-colors">
            Hủy
          </button>
          <button type="button" (click)="onSave()" [disabled]="addressForm.invalid || isSaving" 
                  class="px-6 py-2.5 bg-zinc-900 text-white text-sm font-medium rounded-xl hover:bg-zinc-800 hover:-translate-y-[1px] transition-all disabled:opacity-50 disabled:hover:translate-y-0">
            {{ isSaving ? 'Đang lưu...' : 'Lưu địa chỉ' }}
          </button>
        </div>

      </div>
    </div>
  `
})
export class AddressModalComponent {
  @Input() isOpen = false;
  @Output() closed = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private addressService = inject(AddressService);
  private messageService = inject(MessageService);

  isSaving = false;

  addressForm: FormGroup = this.fb.group({
    receiverName: ['', Validators.required],
    receiverPhone: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
    province: ['', Validators.required],
    city: ['', Validators.required],
    street: ['', Validators.required],
    isDefault: [false]
  });

  close() {
    this.addressForm.reset({ isDefault: false });
    this.closed.emit();
  }

  onSave() {
    if (this.addressForm.invalid) {
      // mark all as touched
      this.addressForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const dto: SaveAddressRequestDto = this.addressForm.value;

    this.addressService.saveAddress(dto).subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.isSuccess) {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã thêm địa chỉ' });
          this.addressForm.reset({ isDefault: false });
          this.saved.emit();
        } else {
          this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: res.message || 'Thêm thất bại' });
        }
      },
      error: () => {
        this.isSaving = false;
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Có lỗi xảy ra' });
      }
    });
  }
}
