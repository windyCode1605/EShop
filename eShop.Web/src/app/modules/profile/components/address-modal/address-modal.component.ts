import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AddressService, SaveAddressRequestDto } from '../../../../core/services/address.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-address-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './address-modal.component.html',
  styleUrl: './address-modal.component.scss'
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
