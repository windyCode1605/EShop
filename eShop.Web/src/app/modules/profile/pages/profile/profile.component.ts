import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService, UserProfile, UpdateProfileDto } from '../../../../core/services/profile.service';
import { AddressService, AddressResponseDto } from '../../../../core/services/address.service';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { AddressModalComponent } from '../../components/address-modal/address-modal.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ToastModule, AddressModalComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private addressService = inject(AddressService);
  private messageService = inject(MessageService);

  profileForm!: FormGroup;
  isSavingProfile = signal(false);

  addresses = signal<AddressResponseDto[]>([]);
  isLoadingAddresses = signal(true);

  isModalOpen = signal(false);

  ngOnInit() {
    this.loadProfile();
    this.loadAddresses();
  }

  loadProfile() {
    this.profileService.getMyProfile().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          const data: UserProfile = res.value;
          this.profileForm = this.fb.group({
            fullName: [data.fullName, Validators.required],
            phoneNumber: [data.phoneNumber, Validators.required]
          });
        }
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải thông tin' });
      }
    });
  }

  loadAddresses() {
    this.isLoadingAddresses.set(true);
    this.addressService.getAddresses().subscribe({
      next: (res) => {
        this.isLoadingAddresses.set(false);
        if (res.isSuccess) {
          this.addresses.set(res.value);
        }
      },
      error: () => {
        this.isLoadingAddresses.set(false);
      }
    });
  }

  onSaveProfile() {
    if (this.profileForm.invalid) return;

    this.isSavingProfile.set(true);
    const dto: UpdateProfileDto = {
      fullName: this.profileForm.value.fullName,
      phoneNumber: this.profileForm.value.phoneNumber,
      dateOfBirth: null, // Bỏ qua nếu ko cần form này
      gender: null,
      avatarUrl: null
    };

    this.profileService.updateMyProfile(dto).subscribe({
      next: (res) => {
        this.isSavingProfile.set(false);
        if (res.isSuccess) {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã cập nhật hồ sơ' });
        } else {
          this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: res.message || 'Cập nhật thất bại' });
        }
      },
      error: () => {
        this.isSavingProfile.set(false);
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Có lỗi xảy ra' });
      }
    });
  }

  openAddressModal() {
    this.isModalOpen.set(true);
  }

  onAddressSaved() {
    this.isModalOpen.set(false);
    this.loadAddresses();
  }

  setAsDefault(id: number) {
    this.addressService.setAsDefault(id).subscribe({
      next: (res) => {
        // reload list
        this.loadAddresses();
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã cập nhật địa chỉ mặc định' });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể thiết lập mặc định' });
      }
    });
  }
}
