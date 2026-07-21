import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService, UserProfile, UpdateProfileDto } from '../../../../core/services/profile.service';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-profile-info',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ToastModule],
  template: `
    <section>
      <div class="flex items-center justify-between mb-8">
        <h2 class="text-xl font-medium tracking-tight">Thông tin chung</h2>
      </div>
      
      <div class="bg-white rounded-[24px] p-8 md:p-10 border border-zinc-200/80 shadow-[0_2px_12px_rgba(0,0,0,0.02)]">
        <form *ngIf="profileForm" [formGroup]="profileForm" (ngSubmit)="onSaveProfile()">
          <div class="grid gap-6">
            
            <div class="form-group group">
              <label class="block text-sm font-medium text-zinc-600 mb-2 group-focus-within:text-zinc-900 transition-colors">Họ và tên</label>
              <input type="text" formControlName="fullName" 
                     class="w-full px-5 py-3.5 rounded-[16px] bg-zinc-50/50 border border-zinc-200 focus:bg-white focus:border-zinc-900 focus:ring-1 focus:ring-zinc-900 outline-none transition-all duration-250" 
                     placeholder="Nhập họ và tên của bạn" />
            </div>

            <div class="form-group group">
              <label class="block text-sm font-medium text-zinc-600 mb-2 group-focus-within:text-zinc-900 transition-colors">Số điện thoại</label>
              <input type="text" formControlName="phoneNumber" 
                     class="w-full px-5 py-3.5 rounded-[16px] bg-zinc-50/50 border border-zinc-200 focus:bg-white focus:border-zinc-900 focus:ring-1 focus:ring-zinc-900 outline-none transition-all duration-250" 
                     placeholder="Nhập số điện thoại" />
            </div>

            <!-- Submit Button -->
            <div class="pt-6 mt-2 border-t border-zinc-100 flex justify-end">
              <button type="submit" [disabled]="profileForm.invalid || isSavingProfile()" 
                      class="w-full sm:w-auto px-8 py-3.5 bg-zinc-900 text-white rounded-[14px] text-sm font-medium hover:bg-zinc-800 hover:scale-[1.02] active:translate-y-[1px] transition-transform duration-250 disabled:opacity-50 disabled:hover:scale-100">
                {{ isSavingProfile() ? 'Đang lưu...' : 'Lưu thay đổi' }}
              </button>
            </div>

          </div>
        </form>

        <!-- Skeleton loading for form -->
        <div *ngIf="!profileForm" class="animate-pulse flex flex-col gap-8">
          <div>
            <div class="h-4 w-20 bg-zinc-100 rounded mb-3"></div>
            <div class="h-14 bg-zinc-100 rounded-[16px]"></div>
          </div>
          <div>
            <div class="h-4 w-24 bg-zinc-100 rounded mb-3"></div>
            <div class="h-14 bg-zinc-100 rounded-[16px]"></div>
          </div>
          <div class="pt-6 mt-2 border-t border-zinc-100 flex justify-end">
            <div class="h-12 w-full sm:w-32 bg-zinc-100 rounded-[14px]"></div>
          </div>
        </div>
      </div>
    </section>
    <p-toast></p-toast>
  `
})
export class ProfileInfoComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private messageService = inject(MessageService);

  profileForm!: FormGroup;
  isSavingProfile = signal(false);

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.profileService.getMyProfile().subscribe({
      next: (res: any) => {
        if (res.isSuccess || res.success) {
          const data: UserProfile | null = res.value || res.data;
          if (data) {
            this.profileForm = this.fb.group({
              fullName: [data.fullName, Validators.required],
              phoneNumber: [data.phoneNumber, Validators.required]
            });
          } else {
            this.initEmptyProfileForm();
          }
        } else if (res.statusCode === 1030) {
          this.initEmptyProfileForm();
        } else {
          this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: res.message || 'Không thể tải thông tin' });
        }
      },
      error: (err) => {
        if (err.error && err.error.statusCode === 1030) {
          this.initEmptyProfileForm();
        } else {
          this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải thông tin' });
        }
      }
    });
  }

  private initEmptyProfileForm() {
    this.profileForm = this.fb.group({
      fullName: ['', Validators.required],
      phoneNumber: ['', Validators.required]
    });
    this.messageService.add({ severity: 'info', summary: 'Thông báo', detail: 'Vui lòng cập nhật thông tin hồ sơ của bạn' });
  }

  onSaveProfile() {
    if (this.profileForm.invalid) return;

    this.isSavingProfile.set(true);
    const dto: UpdateProfileDto = {
      fullName: this.profileForm.value.fullName,
      phoneNumber: this.profileForm.value.phoneNumber,
      dateOfBirth: null,
      gender: null,
      avatarUrl: null
    };

    this.profileService.updateMyProfile(dto).subscribe({
      next: (res: any) => {
        this.isSavingProfile.set(false);
        if (res.isSuccess || res.success) {
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
}
