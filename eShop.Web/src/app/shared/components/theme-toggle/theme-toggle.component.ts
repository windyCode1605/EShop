import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeService } from '../../../core/services/theme.service';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button 
      (click)="toggle()"
      class="relative flex items-center justify-center w-10 h-10 rounded-full bg-surface dark:bg-surface-dark border border-border dark:border-border-dark text-primary dark:text-primary-dark transition-all duration-300 hover:shadow-soft"
      aria-label="Toggle Dark Mode"
    >
      <i class="pi" [ngClass]="isDark() ? 'pi-moon' : 'pi-sun'"></i>
    </button>
  `,
  styles: []
})
export class ThemeToggleComponent {
  private themeService = inject(ThemeService);
  
  isDark = this.themeService.isDark;

  toggle() {
    this.themeService.toggle();
  }
}
