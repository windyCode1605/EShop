import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddressService, AddressResponseDto } from '../../../../core/services/address.service';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { AddressModalComponent } from '../../components/address-modal/address-modal.component';

@Component({
  selector: 'app-profile-address',
  standalone: true,
  imports: [CommonModule, ToastModule, AddressModalComponent],
  template: `
    <section class="flex flex-col gap-8">
      <div class="flex items-center justify-between mb-2 border-b border-zinc-200/60 pb-6">
        <h2 class="text-2xl font-medium tracking-tight text-[#18181B]">Sổ địa chỉ</h2>
        <button type="button" (click)="openAddressModal()" 
                class="text-[13px] font-medium text-[#18181B] bg-white border border-zinc-200 hover:bg-zinc-50 px-4 py-2 rounded-xl flex items-center gap-2 transition-all duration-300 hover:shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
          Thêm địa chỉ
        </button>
      </div>

      <!-- Address List -->
      <div class="flex flex-col gap-5">
        <ng-container *ngIf="!isLoadingAddresses(); else skeletonList">
          
          <ng-container *ngIf="addresses().length > 0; else emptyAddress">
            <div *ngFor="let addr of addresses()" 
                 class="group relative p-6 md:p-8 rounded-[24px] border transition-all duration-300 hover:scale-[1.01] hover:shadow-[0_8px_30px_rgba(0,0,0,0.04)]"
                 [ngClass]="addr.isDefault ? 'bg-white border-[#18181B] shadow-[0_4px_24px_rgba(0,0,0,0.04)]' : 'bg-white/50 border-zinc-200/80 hover:bg-white'">
              
              <div class="flex justify-between items-start mb-4">
                <span class="font-medium text-[#18181B] text-lg tracking-tight">{{ addr.receiverName }}</span>
                <span *ngIf="addr.isDefault" class="px-2.5 py-1 bg-[#18181B] text-white text-[10px] uppercase tracking-widest rounded-[6px] font-semibold shrink-0">MẶC ĐỊNH</span>
              </div>
              
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-3 text-[14px]">
                <div class="flex items-start gap-2">
                  <span class="text-zinc-400 w-[72px] shrink-0 font-medium">Tỉnh:</span>
                  <span class="text-zinc-900 truncate" [title]="addr.province">{{ addr.province }}</span>
                </div>
                <div class="flex items-start gap-2">
                  <span class="text-zinc-400 w-[72px] shrink-0 font-medium">Thành phố:</span>
                  <span class="text-zinc-900 truncate" [title]="addr.city">{{ addr.city || '---' }}</span>
                </div>
                <div class="flex items-start gap-2">
                  <span class="text-zinc-400 w-[72px] shrink-0 font-medium">Đường:</span>
                  <span class="text-zinc-900 truncate" [title]="addr.street">{{ addr.street }}</span>
                </div>
                <div class="flex items-start gap-2">
                  <span class="text-zinc-400 w-[72px] shrink-0 font-medium">Sđt:</span>
                  <span class="text-zinc-900 font-medium tracking-wide">{{ addr.receiverPhone }}</span>
                </div>
              </div>

              <div class="mt-6 pt-4 border-t border-zinc-100 flex items-center gap-4 text-sm font-medium">
                <button *ngIf="!addr.isDefault" (click)="setAsDefault(addr.id)" 
                        class="text-zinc-400 hover:text-[#18181B] transition-colors duration-250 hover:underline underline-offset-4 decoration-zinc-300">
                  Đặt làm mặc định
                </button>
              </div>
            </div>
          </ng-container>

        </ng-container>
      </div>
    </section>

    <!-- Empty State -->
    <ng-template #emptyAddress>
      <div class="py-20 px-8 border border-zinc-200 border-dashed rounded-[24px] text-center bg-white/50 flex flex-col items-center justify-center">
        <div class="w-14 h-14 rounded-full bg-white border border-zinc-200 flex items-center justify-center text-zinc-400 mb-5 shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
        </div>
        <h3 class="text-xl tracking-tight text-[#18181B] font-medium mb-2">Chưa có địa chỉ</h3>
        <p class="text-[15px] text-zinc-500 mb-8 max-w-[250px] leading-relaxed">Thêm địa chỉ giao hàng để trải nghiệm mua sắm nhanh chóng hơn.</p>
        <button type="button" (click)="openAddressModal()" class="px-8 py-3.5 bg-[#18181B] text-white text-[14px] font-medium rounded-[14px] hover:bg-black hover:scale-[1.02] hover:shadow-lg transition-all duration-300">
          Thêm địa chỉ mới
        </button>
      </div>
    </ng-template>

    <!-- Skeleton -->
    <ng-template #skeletonList>
      <div class="animate-pulse flex flex-col gap-4">
        <div class="h-[180px] bg-zinc-100 rounded-[24px] w-full"></div>
        <div class="h-[180px] bg-zinc-100 rounded-[24px] w-full"></div>
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
  `
})
export class ProfileAddressComponent implements OnInit {
  private addressService = inject(AddressService);
  private messageService = inject(MessageService);

  addresses = signal<AddressResponseDto[]>([]);
  isLoadingAddresses = signal(true);
  isModalOpen = signal(false);

  ngOnInit() {
    this.loadAddresses();
  }

  loadAddresses() {
    this.isLoadingAddresses.set(true);
    this.addressService.getAddresses().subscribe({
      next: (res: any) => {
        this.isLoadingAddresses.set(false);
        if (res.isSuccess || res.success) {
          const raw = res.value || res.data || [];
          this.addresses.set([...raw]); // Tạo tham chiếu mảng mới để signal update UI
        }
      },
      error: () => {
        this.isLoadingAddresses.set(false);
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
    // Optimistic UI update: Set directly to avoid UI delay
    const currentList = this.addresses();
    const updatedList = currentList.map(a => ({
      ...a,
      isDefault: a.id === id
    }));
    this.addresses.set(updatedList);

    this.addressService.setAsDefault(id).subscribe({
      next: (res: any) => {
        if (res.isSuccess || res.success) {
          this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đã cập nhật địa chỉ mặc định' });
          // Fetch from server again to ensure data consistency
          this.loadAddresses();
        } else {
          // Revert on failure
          this.loadAddresses();
          this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: res.message || 'Không thể thiết lập mặc định' });
        }
      },
      error: () => {
        this.loadAddresses(); // Revert on failure
        this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể thiết lập mặc định' });
      }
    });
  }
}
