import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  styles: [`
    .overlay {
      position: fixed;
      inset: 0;
      background: rgba(24,24,27,0.45);
      backdrop-filter: blur(4px);
      z-index: 1000;
      display: flex;
      align-items: center;
      justify-content: center;
      animation: fadeIn 150ms ease;
    }

    @keyframes fadeIn {
      from { opacity: 0; }
      to   { opacity: 1; }
    }

    .dialog {
      background: #FFFFFF;
      border: 1px solid #E4E4E7;
      border-radius: 24px;
      padding: 32px;
      width: 100%;
      max-width: 400px;
      margin: 16px;
      animation: slideUp 200ms cubic-bezier(0.34,1.56,0.64,1);
    }

    @keyframes slideUp {
      from { opacity: 0; transform: translateY(12px) scale(0.97); }
      to   { opacity: 1; transform: translateY(0) scale(1); }
    }

    .icon-wrap {
      width: 48px;
      height: 48px;
      border-radius: 14px;
      background: #FEF2F2;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 16px;
      color: #EF4444;
    }

    .title {
      font-size: 18px;
      font-weight: 600;
      color: #18181B;
      margin: 0 0 8px;
      letter-spacing: -0.3px;
    }

    .message {
      font-size: 14px;
      color: #71717A;
      margin: 0 0 24px;
      line-height: 1.6;
    }

    .actions {
      display: flex;
      gap: 12px;
    }

    .btn {
      flex: 1;
      height: 44px;
      border-radius: 14px;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      border: none;
      transition: all 150ms ease;
    }

    .btn-cancel {
      background: #F4F4F5;
      color: #18181B;
    }

    .btn-cancel:hover {
      background: #E4E4E7;
    }

    .btn-danger {
      background: #18181B;
      color: #FFFFFF;
    }

    .btn-danger:hover {
      background: #EF4444;
    }

    .btn:active {
      transform: translateY(1px);
    }
  `],
  template: `
    <div class="overlay" (click)="onCancel()" *ngIf="visible">
      <div class="dialog" (click)="$event.stopPropagation()" role="dialog" aria-modal="true">
        <div class="icon-wrap">
          <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24"
               fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/>
            <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/>
          </svg>
        </div>
        <h3 class="title">{{ title }}</h3>
        <p class="message">{{ message }}</p>
        <div class="actions">
          <button class="btn btn-cancel" (click)="onCancel()" id="confirm-dialog-cancel">Hủy</button>
          <button class="btn btn-danger" (click)="onConfirm()" id="confirm-dialog-confirm">{{ confirmLabel }}</button>
        </div>
      </div>
    </div>
  `
})
export class ConfirmDialogComponent {
  @Input() visible = false;
  @Input() title = 'Xác nhận xóa';
  @Input() message = 'Hành động này không thể hoàn tác. Bạn có chắc muốn tiếp tục?';
  @Input() confirmLabel = 'Xóa';

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
