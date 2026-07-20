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
  template: `
    <div class="profile-page min-h-[100dvh] bg-[#F9FAFB] py-12 md:py-24">
      <div class="max-w-5xl mx-auto px-6">
        
        <!-- Header -->
        <header class="mb-12">
          <h1 class="text-3xl font-medium tracking-tight text-zinc-900 mb-2">Hồ sơ cá nhân</h1>
          <p class="text-zinc-500">Quản lý thông tin tài khoản và địa chỉ giao hàng của bạn.</p>
        </header>

        <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-16">
          
          <!-- L Cột: Thông tin cá nhân (7 cols) -->
          <div class="lg:col-span-7">
            <section class="bg-white rounded-[24px] p-8 border border-zinc-200">
              <h2 class="text-lg font-medium text-zinc-900 mb-6">Thông tin chung</h2>
              
              <form *ngIf="profileForm" [formGroup]="profileForm" (ngSubmit)="onSaveProfile()">
                <div class="grid gap-6">
                  
                  <div class="form-group">
                    <label class="block text-sm font-medium text-zinc-700 mb-2">Họ và tên</label>
                    <input type="text" formControlName="fullName" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors" placeholder="Nguyễn Văn A" />
                  </div>

                  <div class="form-group">
                    <label class="block text-sm font-medium text-zinc-700 mb-2">Số điện thoại</label>
                    <input type="text" formControlName="phoneNumber" class="w-full px-4 py-3 rounded-2xl bg-zinc-50 border border-zinc-200 focus:bg-white focus:border-zinc-400 focus:ring-0 outline-none transition-colors" placeholder="09xxxx" />
                  </div>

                  <!-- Submit -->
                  <div class="pt-4">
                    <button type="submit" [disabled]="profileForm.invalid || isSavingProfile()" 
                            class="w-full sm:w-auto px-6 py-3 bg-zinc-900 text-white rounded-2xl font-medium hover:bg-zinc-800 hover:-translate-y-[1px] transition-all disabled:opacity-50 disabled:hover:translate-y-0">
                      {{ isSavingProfile() ? 'Đang lưu...' : 'Lưu thay đổi' }}
                    </button>
                  </div>

                </div>
              </form>

              <!-- Skeleton loading cho form -->
              <div *ngIf="!profileForm" class="animate-pulse flex flex-col gap-6">
                <div class="h-12 bg-zinc-100 rounded-2xl"></div>
                <div class="h-12 bg-zinc-100 rounded-2xl"></div>
                <div class="h-12 w-32 bg-zinc-100 rounded-2xl mt-4"></div>
              </div>
            </section>
          </div>

          <!-- R Cột: Địa chỉ (5 cols) -->
          <div class="lg:col-span-5">
            <section class="flex flex-col gap-6">
              
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-medium text-zinc-900">Sổ địa chỉ</h2>
                <button type="button" (click)="openAddressModal()" 
                        class="text-sm font-medium text-zinc-900 flex items-center gap-2 hover:text-zinc-600 transition-colors">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                  Thêm địa chỉ
                </button>
              </div>

              <!-- Danh sách địa chỉ -->
              <div class="flex flex-col gap-4">
                <ng-container *ngIf="!isLoadingAddresses(); else skeletonList">
                  
                  <ng-container *ngIf="addresses().length > 0; else emptyAddress">
                    <div *ngFor="let addr of addresses()" 
                         class="address-card relative p-6 rounded-[20px] border transition-all"
                         [ngClass]="addr.isDefault ? 'bg-white border-zinc-900' : 'bg-white border-zinc-200'">
                      
                      <div class="flex justify-between items-start mb-3">
                        <div class="flex items-center gap-3">
                          <span class="font-medium text-zinc-900">{{ addr.receiverName }}</span>
                          <span class="text-sm text-zinc-500">{{ addr.receiverPhone }}</span>
                        </div>
                        <span *ngIf="addr.isDefault" class="px-2 py-1 bg-zinc-900 text-white text-[10px] uppercase tracking-wider rounded-md font-semibold">Mặc định</span>
                      </div>
                      
                      <p class="text-sm text-zinc-600 leading-relaxed max-w-[90%]">
                        {{ addr.street }}<br/>
                        {{ addr.city ? addr.city + ', ' : '' }}{{ addr.province }}
                      </p>

                      <div class="mt-5 flex items-center gap-4 text-sm font-medium">
                        <button *ngIf="!addr.isDefault" (click)="setAsDefault(addr.id)" class="text-zinc-500 hover:text-zinc-900 transition-colors">Đặt mặc định</button>
                      </div>
                    </div>
                  </ng-container>

                </ng-container>
              </div>

            </section>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Empty State -->
    <ng-template #emptyAddress>
      <div class="py-12 px-6 border border-zinc-200 border-dashed rounded-[20px] text-center bg-transparent">
        <p class="text-sm text-zinc-500 mb-4">Bạn chưa có địa chỉ giao hàng nào.</p>
        <button type="button" (click)="openAddressModal()" class="text-sm font-medium text-zinc-900 underline underline-offset-4 decoration-zinc-300 hover:decoration-zinc-900 transition-all">Thêm địa chỉ đầu tiên</button>
      </div>
    </ng-template>

    <!-- Skeleton -->
    <ng-template #skeletonList>
      <div class="animate-pulse flex flex-col gap-4">
        <div class="h-32 bg-zinc-100 rounded-[20px] w-full"></div>
        <div class="h-32 bg-zinc-100 rounded-[20px] w-full"></div>
      </div>
    </ng-template>

    <!-- Toast -->
    <p-toast></p-toast>

    <!-- Modal thêm địa chỉ -->
    <app-address-modal 
      [isOpen]="isModalOpen()" 
      (closed)="isModalOpen.set(false)"
      (saved)="onAddressSaved()">
    </app-address-modal>
  `,
  styles: [`
    /* Bỏ font-family nếu đã định nghĩa ở global */
  `]
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
