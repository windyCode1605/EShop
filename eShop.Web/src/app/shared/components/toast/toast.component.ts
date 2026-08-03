import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, style, animate, transition } from '@angular/animations';
import { ToastService, ToastItem, ToastPosition } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  animations: [
    trigger('toastAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(12px) scale(0.96)' }),
        animate('250ms cubic-bezier(0.16, 1, 0.3, 1)', style({ opacity: 1, transform: 'translateY(0) scale(1)' }))
      ]),
      transition(':leave', [
        animate('200ms cubic-bezier(0.4, 0, 1, 1)', style({ opacity: 0, transform: 'translateY(8px) scale(0.95)' }))
      ])
    ])
  ],
  styles: [`
    :host {
      pointer-events: none;
      position: fixed;
      inset: 0;
      z-index: 9999;
      display: block;
    }

    .toast-interactive {
      pointer-events: auto;
    }

    @keyframes shrink {
      from { width: 100%; }
      to { width: 0%; }
    }

    .progress-bar-anim {
      animation: shrink linear forwards;
    }
  `],
  template: `
    <!-- Top Left Container -->
    <div class="fixed top-5 left-5 z-[9999] flex flex-col gap-3 max-w-[420px] w-full pointer-events-none" *ngIf="topLeftToasts().length > 0">
      <ng-container *ngFor="let toast of topLeftToasts(); trackBy: trackById">
        <ng-container *ngTemplateOutlet="toastCard; context: { $implicit: toast }"></ng-container>
      </ng-container>
    </div>

    <!-- Top Center Container -->
    <div class="fixed top-5 left-1/2 -translate-x-1/2 z-[9999] flex flex-col gap-3 max-w-[420px] w-full pointer-events-none items-center" *ngIf="topCenterToasts().length > 0">
      <ng-container *ngFor="let toast of topCenterToasts(); trackBy: trackById">
        <ng-container *ngTemplateOutlet="toastCard; context: { $implicit: toast }"></ng-container>
      </ng-container>
    </div>

    <!-- Top Right Container -->
    <div class="fixed top-5 right-5 z-[9999] flex flex-col gap-3 max-w-[420px] w-full pointer-events-none items-end" *ngIf="topRightToasts().length > 0">
      <ng-container *ngFor="let toast of topRightToasts(); trackBy: trackById">
        <ng-container *ngTemplateOutlet="toastCard; context: { $implicit: toast }"></ng-container>
      </ng-container>
    </div>

    <!-- Bottom Left Container -->
    <div class="fixed bottom-5 left-5 z-[9999] flex flex-col-reverse gap-3 max-w-[420px] w-full pointer-events-none" *ngIf="bottomLeftToasts().length > 0">
      <ng-container *ngFor="let toast of bottomLeftToasts(); trackBy: trackById">
        <ng-container *ngTemplateOutlet="toastCard; context: { $implicit: toast }"></ng-container>
      </ng-container>
    </div>

    <!-- Bottom Center Container -->
    <div class="fixed bottom-5 left-1/2 -translate-x-1/2 z-[9999] flex flex-col-reverse gap-3 max-w-[420px] w-full pointer-events-none items-center" *ngIf="bottomCenterToasts().length > 0">
      <ng-container *ngFor="let toast of bottomCenterToasts(); trackBy: trackById">
        <ng-container *ngTemplateOutlet="toastCard; context: { $implicit: toast }"></ng-container>
      </ng-container>
    </div>

    <!-- Bottom Right Container (Default) -->
    <div class="fixed bottom-5 right-5 z-[9999] flex flex-col-reverse gap-3 max-w-[420px] w-full pointer-events-none items-end" *ngIf="bottomRightToasts().length > 0">
      <ng-container *ngFor="let toast of bottomRightToasts(); trackBy: trackById">
        <ng-container *ngTemplateOutlet="toastCard; context: { $implicit: toast }"></ng-container>
      </ng-container>
    </div>

    <!-- Reusable Toast Card Template -->
    <ng-template #toastCard let-toast>
      <div [@toastAnimation]
           class="toast-interactive relative overflow-hidden w-full max-w-[400px] rounded-2xl p-4 shadow-2xl backdrop-blur-xl transition-all duration-200"
           [ngClass]="getContainerClasses(toast.type)">

        <!-- Left Accent Line & Icon -->
        <div class="flex items-start gap-3.5">
          <!-- Icon Circle -->
          <div class="w-8 h-8 rounded-full flex items-center justify-center shrink-0 mt-0.5"
               [ngClass]="getIconBgClasses(toast.type)">
            <!-- Success Icon -->
            <svg *ngIf="toast.type === 'success'" class="w-4 h-4 text-emerald-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M20 6 9 17l-5-5"/>
            </svg>

            <!-- Error Icon -->
            <svg *ngIf="toast.type === 'error'" class="w-4 h-4 text-rose-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="12" cy="12" r="10"/>
              <path d="m15 9-6 6"/>
              <path d="m9 9 6 6"/>
            </svg>

            <!-- Warning Icon -->
            <svg *ngIf="toast.type === 'warning'" class="w-4 h-4 text-amber-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"/>
              <path d="M12 9v4"/>
              <path d="M12 17h.01"/>
            </svg>

            <!-- Info Icon -->
            <svg *ngIf="toast.type === 'info'" class="w-4 h-4 text-sky-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="12" cy="12" r="10"/>
              <path d="M12 16v-4"/>
              <path d="M12 8h.01"/>
            </svg>
          </div>

          <!-- Content Text -->
          <div class="flex-1 min-w-0 pr-2">
            <div class="flex items-center gap-2 mb-0.5">
              <h4 class="text-[13px] font-semibold tracking-tight text-white/95" *ngIf="toast.title">
                {{ toast.title }}
              </h4>
              <span *ngIf="toast.statusCode" class="text-[10px] font-mono px-1.5 py-0.2 rounded bg-white/10 text-white/70">
                HTTP {{ toast.statusCode }}
              </span>
            </div>
            <p class="text-[13px] leading-relaxed text-zinc-300 font-normal break-words">
              {{ toast.message }}
            </p>
          </div>

          <!-- Close Button -->
          <button *ngIf="toast.dismissible"
                  (click)="dismiss(toast.id)"
                  class="w-6 h-6 rounded-lg flex items-center justify-center text-zinc-400 hover:text-white hover:bg-white/10 transition-colors shrink-0 -mr-1 -mt-0.5">
            <svg class="w-3.5 h-3.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M18 6 6 18"/><path d="m6 6 12 12"/>
            </svg>
          </button>
        </div>

        <!-- Progress Bar Auto-Dismiss Countdown -->
        <div *ngIf="toast.duration > 0"
             class="absolute bottom-0 left-0 right-0 h-[2px] opacity-70">
          <div class="h-full progress-bar-anim"
               [ngClass]="getProgressBarClasses(toast.type)"
               [style.animation-duration.ms]="toast.duration"></div>
        </div>

      </div>
    </ng-template>
  `
})
export class ToastComponent {
  readonly toastService = inject(ToastService);

  readonly allToasts = this.toastService.toasts;

  // Filtered computed signals for position layout
  readonly topRightToasts = computed(() => this.allToasts().filter(t => t.position === 'top-right'));
  readonly topLeftToasts = computed(() => this.allToasts().filter(t => t.position === 'top-left'));
  readonly topCenterToasts = computed(() => this.allToasts().filter(t => t.position === 'top-center'));
  readonly bottomRightToasts = computed(() => this.allToasts().filter(t => t.position === 'bottom-right' || !t.position));
  readonly bottomLeftToasts = computed(() => this.allToasts().filter(t => t.position === 'bottom-left'));
  readonly bottomCenterToasts = computed(() => this.allToasts().filter(t => t.position === 'bottom-center'));

  dismiss(id: string): void {
    this.toastService.dismiss(id);
  }

  trackById(_index: number, item: ToastItem): string {
    return item.id;
  }

  getContainerClasses(type: string): string {
    switch (type) {
      case 'success':
        return 'bg-zinc-900/90 border border-emerald-500/25 shadow-emerald-950/20';
      case 'error':
        return 'bg-zinc-900/90 border border-rose-500/25 shadow-rose-950/20';
      case 'warning':
        return 'bg-zinc-900/90 border border-amber-500/25 shadow-amber-950/20';
      case 'info':
      default:
        return 'bg-zinc-900/90 border border-sky-500/25 shadow-sky-950/20';
    }
  }

  getIconBgClasses(type: string): string {
    switch (type) {
      case 'success':
        return 'bg-emerald-500/15 ring-1 ring-emerald-500/30';
      case 'error':
        return 'bg-rose-500/15 ring-1 ring-rose-500/30';
      case 'warning':
        return 'bg-amber-500/15 ring-1 ring-amber-500/30';
      case 'info':
      default:
        return 'bg-sky-500/15 ring-1 ring-sky-500/30';
    }
  }

  getProgressBarClasses(type: string): string {
    switch (type) {
      case 'success':
        return 'bg-emerald-400';
      case 'error':
        return 'bg-rose-400';
      case 'warning':
        return 'bg-amber-400';
      case 'info':
      default:
        return 'bg-sky-400';
    }
  }
}
