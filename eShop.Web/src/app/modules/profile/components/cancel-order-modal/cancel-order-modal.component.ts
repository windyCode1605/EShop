import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface CancelOrderResult {
  orderId: number;
  reason: string;
}

@Component({
  selector: 'app-cancel-order-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="isOpen" class="fixed inset-0 z-[110] flex items-center justify-center p-4 sm:p-6" (click)="close()">
      
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-zinc-900/40 backdrop-blur-sm transition-opacity"></div>
      
      <!-- Modal Content -->
      <div class="relative w-full max-w-md bg-white rounded-[24px] shadow-2xl overflow-hidden flex flex-col" (click)="$event.stopPropagation()">
        
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-5 border-b border-zinc-100 bg-white">
          <h3 class="text-xl font-medium tracking-tight text-zinc-900">Hủy đơn hàng</h3>
          <button type="button" (click)="close()" [disabled]="isSubmitting" class="p-2 text-zinc-400 hover:text-zinc-900 hover:bg-zinc-100 rounded-full transition-colors disabled:opacity-50">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>

        <!-- Body -->
        <div class="p-6">
          <p class="text-sm text-zinc-600 mb-6 leading-relaxed">
            Bạn đang yêu cầu hủy đơn hàng <span class="font-semibold text-zinc-900">{{ order?.orderCode }}</span>. Vui lòng chọn lý do hủy đơn:
          </p>

          <div class="flex flex-col gap-3">
            <label *ngFor="let reason of predefinedReasons" 
                   class="flex items-center gap-3 p-3.5 rounded-xl border cursor-pointer transition-colors"
                   [ngClass]="selectedReason === reason ? 'border-zinc-900 bg-zinc-50' : 'border-zinc-200 hover:border-zinc-300 bg-white'">
              <div class="w-5 h-5 rounded-full border flex items-center justify-center shrink-0 transition-colors"
                   [ngClass]="selectedReason === reason ? 'border-zinc-900' : 'border-zinc-300'">
                 <div *ngIf="selectedReason === reason" class="w-2.5 h-2.5 rounded-full bg-zinc-900"></div>
              </div>
              <input type="radio" [value]="reason" [(ngModel)]="selectedReason" class="hidden" [disabled]="isSubmitting">
              <span class="text-sm font-medium text-zinc-900">{{ reason }}</span>
            </label>

            <!-- Other Reason -->
            <label class="flex items-center gap-3 p-3.5 rounded-xl border cursor-pointer transition-colors"
                   [ngClass]="selectedReason === 'OTHER' ? 'border-zinc-900 bg-zinc-50' : 'border-zinc-200 hover:border-zinc-300 bg-white'">
              <div class="w-5 h-5 rounded-full border flex items-center justify-center shrink-0 transition-colors"
                   [ngClass]="selectedReason === 'OTHER' ? 'border-zinc-900' : 'border-zinc-300'">
                 <div *ngIf="selectedReason === 'OTHER'" class="w-2.5 h-2.5 rounded-full bg-zinc-900"></div>
              </div>
              <input type="radio" value="OTHER" [(ngModel)]="selectedReason" class="hidden" [disabled]="isSubmitting">
              <span class="text-sm font-medium text-zinc-900">Khác</span>
            </label>

            <!-- Custom Reason Textarea -->
            <div *ngIf="selectedReason === 'OTHER'" class="mt-2 animate-in fade-in slide-in-from-top-2 duration-200">
              <textarea [(ngModel)]="customReason" 
                        [disabled]="isSubmitting"
                        rows="3" 
                        placeholder="Vui lòng nhập lý do hủy đơn (tùy chọn)..."
                        class="w-full px-4 py-3 rounded-xl border border-zinc-200 text-sm text-zinc-900 placeholder:text-zinc-400 focus:outline-none focus:border-zinc-900 focus:ring-1 focus:ring-zinc-900 transition-shadow resize-none disabled:bg-zinc-50 disabled:text-zinc-500"></textarea>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-6 pt-2 flex flex-col sm:flex-row-reverse gap-3 mt-auto">
          <button type="button" 
                  (click)="submit()" 
                  [disabled]="!isValid() || isSubmitting"
                  class="px-6 py-3 w-full sm:w-auto bg-red-600 text-white text-sm font-medium rounded-xl hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2">
            <svg *ngIf="isSubmitting" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Xác nhận hủy đơn
          </button>
          <button type="button" 
                  (click)="close()" 
                  [disabled]="isSubmitting"
                  class="px-6 py-3 w-full sm:w-auto bg-white border border-zinc-200 text-zinc-700 text-sm font-medium rounded-xl hover:bg-zinc-50 hover:text-zinc-900 transition-colors disabled:opacity-50">
            Không, giữ lại đơn
          </button>
        </div>
      </div>
    </div>
  `
})
export class CancelOrderModalComponent {
  @Input() isOpen = false;
  @Input() order: any = null;
  @Input() isSubmitting = false;
  
  @Output() closed = new EventEmitter<void>();
  @Output() confirmed = new EventEmitter<CancelOrderResult>();

  predefinedReasons = [
    'Thay đổi ý định mua',
    'Phí vận chuyển quá cao',
    'Thời gian giao hàng quá lâu',
    'Tìm thấy giá rẻ hơn ở nơi khác'
  ];

  selectedReason: string = '';
  customReason: string = '';

  close() {
    if (this.isSubmitting) return;
    this.closed.emit();
    // Reset state after a short delay for animation
    setTimeout(() => {
      this.selectedReason = '';
      this.customReason = '';
    }, 300);
  }

  isValid(): boolean {
    if (!this.selectedReason) return false;
    if (this.selectedReason === 'OTHER' && !this.customReason.trim()) return false;
    return true;
  }

  submit() {
    if (!this.isValid() || this.isSubmitting || !this.order) return;
    
    const finalReason = this.selectedReason === 'OTHER' 
      ? this.customReason.trim() 
      : this.selectedReason;
      
    this.confirmed.emit({
      orderId: this.order.id,
      reason: finalReason
    });
  }
}
