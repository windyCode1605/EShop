import { Injectable, signal, effect } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  /** true = dark mode, false = light mode */
  isDark = signal<boolean>(this._loadPreference());

  constructor() {
    // Whenever isDark changes, apply the class to <html> and persist
    effect(() => {
      const dark = this.isDark();
      const html = document.documentElement;
      if (dark) {
        html.classList.add('dark');
        html.classList.remove('light');
      } else {
        html.classList.remove('dark');
        html.classList.add('light');
      }
      localStorage.setItem('mqn-theme', dark ? 'dark' : 'light');
    });
  }

  toggle(): void {
    this.isDark.update(v => !v);
  }

  private _loadPreference(): boolean {
    const saved = localStorage.getItem('mqn-theme');
    if (saved) return saved === 'dark';
    // Default: dark for now (admin prefers dark)
    return true;
  }
}
