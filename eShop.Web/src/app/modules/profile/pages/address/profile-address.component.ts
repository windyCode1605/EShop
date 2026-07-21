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
    <section class="flex flex-col gap-6">
      <div class="flex items-center justify-between mb-2">
        <h2 class="text-xl font-medium tracking-tight">Sổ địa chỉ</h2>
        <button type="button" (click)="openAddressModal()" 
                class="text-sm font-medium text-zinc-500 hover:text-zinc-900 flex items-center gap-2 transition-colors duration-250">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
          Thêm mới
        </button>
      </div>

      <!-- Address List -->
      <div class="flex flex-col gap-4">
        <ng-container *ngIf="!isLoadingAddresses(); else skeletonList">
          
          <ng-container *ngIf="addresses().length > 0; else emptyAddress">
            <div *ngFor="let addr of addresses()" 
                 class="group relative p-5 md:p-6 rounded-[20px] border transition-all duration-300 hover:scale-[1.01] hover:shadow-sm"
                 [ngClass]="addr.isDefault ? 'bg-white border-zinc-900 shadow-[0_2px_12px_rgba(0,0,0,0.03)]' : 'bg-white/50 border-zinc-200/80 hover:bg-white'">
              
              <div class="flex justify-between items-start mb-3">
                <span class="font-medium text-zinc-900 text-base">{{ addr.receiverName }}</span>
                <span *ngIf="addr.isDefault" class="px-2 py-1 bg-zinc-900 text-white text-[10px] uppercase tracking-[0.1em] rounded-md font-semibold shrink-0 transition-opacity">MẶC ĐỊNH</span>
              </div>
              
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-2 text-sm">
                <div class="flex items-center gap-2">
                  <span class="text-zinc-500 w-16 shrink-0">Tỉnh:</span>
                  <span class="text-zinc-900 truncate" [title]="addr.province">{{ addr.province }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-zinc-500 w-[72px] shrink-0">Thành phố:</span>
                  <span class="text-zinc-900 truncate" [title]="addr.city">{{ addr.city || '---' }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-zinc-500 w-16 shrink-0">Đường:</span>
                  <span class="text-zinc-900 truncate" [title]="addr.street">{{ addr.street }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-zinc-500 w-[72px] shrink-0">Sđt:</span>
                  <span class="text-zinc-900 truncate">{{ addr.receiverPhone }}</span>
                </div>
              </div>

              <div class="mt-4 flex items-center gap-4 text-sm font-medium">
                <button *ngIf="!addr.isDefault" (click)="setAsDefault(addr.id)" 
                        class="text-zinc-400 hover:text-zinc-900 transition-colors duration-250 underline underline-offset-4 decoration-transparent hover:decoration-zinc-300">
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
      <div class="py-16 px-8 border border-zinc-200 border-dashed rounded-[24px] text-center bg-zinc-50/50 flex flex-col items-center justify-center">
        <div class="w-12 h-12 rounded-full bg-white border border-zinc-200 flex items-center justify-center text-zinc-400 mb-4 shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
        </div>
        <h3 class="text-zinc-900 font-medium mb-1">Chưa có địa chỉ</h3>
        <p class="text-sm text-zinc-500 mb-6 max-w-[200px] leading-relaxed">Thêm địa chỉ giao hàng để thanh toán nhanh chóng hơn.</p>
        <button type="button" (click)="openAddressModal()" class="px-6 py-2.5 bg-zinc-900 text-white text-sm font-medium rounded-[14px] hover:bg-zinc-800 hover:scale-[1.02] transition-transform duration-250">
          Thêm địa chỉ
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
