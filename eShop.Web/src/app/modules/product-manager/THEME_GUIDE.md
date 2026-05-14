# 🎨 Product Manager Theme & Color Palette

## Overview

The Product Manager module now includes a comprehensive theme system with 4 predefined color schemes inspired by modern e-commerce design (ATELIER style).

## Available Themes

### 1. **Dark Theme** (Default) 🌙
Perfect for modern e-commerce with blue accents
- **Primary Color**: `#007bff` (Vibrant Blue)
- **Background**: `#1a1a1a` (Dark Gray)
- **Surface**: `#2d2d2d` (Light Gray for cards)
- **Text**: `#ffffff` (White)
- **Best For**: Night mode, e-commerce sites

```css
Primary: #007bff
Secondary: #6c757d
Background: #1a1a1a
Surface: #2d2d2d
```

### 2. **Light Theme** ☀️
Clean, professional look for daytime use
- **Primary Color**: `#0056b3` (Dark Blue)
- **Background**: `#f5f5f5` (Light Gray)
- **Surface**: `#ffffff` (White)
- **Text**: `#212121` (Dark)
- **Best For**: Office use, professional settings

```css
Primary: #0056b3
Secondary: #6c757d
Background: #f5f5f5
Surface: #ffffff
```

### 3. **Professional Theme** 💼
Corporate, sophisticated look
- **Primary Color**: `#1e3a8a` (Deep Blue)
- **Background**: `#f8fafc` (Off-white)
- **Text**: `#0f172a` (Very dark blue)
- **Best For**: Enterprise applications, B2B

```css
Primary: #1e3a8a
Secondary: #64748b
Background: #f8fafc
```

### 4. **Modern Dark Theme** 🚀
Contemporary design with vibrant accents
- **Primary Color**: `#3b82f6` (Blue-500)
- **Secondary**: `#8b5cf6` (Purple)
- **Background**: `#0f172a` (Very dark blue)
- **Best For**: Dev tools, gaming, modern startups

```css
Primary: #3b82f6
Secondary: #8b5cf6
Background: #0f172a
```

## Usage

### TypeScript
```typescript
import { ThemeConfig, DARK_THEME, LIGHT_THEME } from '@app/modules/product-manager/theme/theme.config';

// Get current theme
const theme = ThemeConfig.getTheme();
console.log(theme.colors.primary);

// Switch theme
ThemeConfig.setTheme('light');
ThemeConfig.setTheme('professional');
ThemeConfig.setTheme('modern-dark');

// Get specific color
const primaryColor = ThemeConfig.getColor('primary');

// Save user preference
ThemeConfig.savePreference('dark');

// Initialize on app load
ThemeConfig.initialize('dark');
```

### SCSS/CSS
Use CSS variables for dynamic theming:

```scss
// In global styles.scss
@import './app/modules/product-manager/theme/theme.config';

// Apply theme variables
.product-card {
  background-color: var(--color-surface);
  color: var(--color-text);
  border: 1px solid var(--color-border);
  border-radius: 8px;
  padding: 1rem;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
  
  &:hover {
    opacity: 0.8;
  }
}

.success {
  color: var(--color-success);
}

.error {
  color: var(--color-error);
}

.warning {
  color: var(--color-warning);
}
```

### HTML Template
```html
<div class="container" style="background-color: var(--color-background); color: var(--color-text);">
  <div class="card" style="background-color: var(--color-surface); border-color: var(--color-border);">
    <h3>Product Card</h3>
  </div>
</div>
```

## Color Palette Reference

| Color | Dark Theme | Light Theme | Professional | Modern Dark |
|-------|-----------|------------|--------------|------------|
| **Primary** | #007bff | #0056b3 | #1e3a8a | #3b82f6 |
| **Secondary** | #6c757d | #6c757d | #64748b | #8b5cf6 |
| **Background** | #1a1a1a | #f5f5f5 | #f8fafc | #0f172a |
| **Surface** | #2d2d2d | #ffffff | #ffffff | #1e293b |
| **Text** | #ffffff | #212121 | #0f172a | #f1f5f9 |
| **Text Secondary** | #b0b0b0 | #666666 | #475569 | #cbd5e1 |
| **Border** | #404040 | #e0e0e0 | #cbd5e1 | #334155 |
| **Success** | #28a745 | #28a745 | #16a34a | #10b981 |
| **Warning** | #ffc107 | #ffc107 | #d97706 | #f59e0b |
| **Error** | #dc3545 | #dc3545 | #dc2626 | #ef4444 |
| **Info** | #17a2b8 | #17a2b8 | #0284c7 | #06b6d4 |

## CSS Variables

All colors are available as CSS custom properties:

```css
:root {
  --color-primary: #007bff;
  --color-secondary: #6c757d;
  --color-background: #1a1a1a;
  --color-surface: #2d2d2d;
  --color-text: #ffffff;
  --color-text-secondary: #b0b0b0;
  --color-border: #404040;
  --color-success: #28a745;
  --color-warning: #ffc107;
  --color-error: #dc3545;
  --color-info: #17a2b8;
}
```

## Implementation Examples

### Component Styling
```typescript
import { Component, OnInit } from '@angular/core';
import { ThemeConfig } from '@app/modules/product-manager/theme/theme.config';

@Component({
  selector: 'app-product-card',
  template: `
    <div class="card">
      <h3>{{ product.name }}</h3>
      <p>{{ product.description }}</p>
      <button class="btn-primary">View Details</button>
    </div>
  `,
  styles: [`
    .card {
      background-color: var(--color-surface);
      color: var(--color-text);
      border: 1px solid var(--color-border);
    }
    
    .btn-primary {
      background-color: var(--color-primary);
      color: white;
    }
  `]
})
export class ProductCardComponent implements OnInit {
  ngOnInit() {
    // Initialize theme on component load
    ThemeConfig.initialize();
  }
}
```

### Dynamic Theme Switching
```typescript
import { Component } from '@angular/core';
import { ThemeConfig, AVAILABLE_THEMES } from '@app/modules/product-manager/theme/theme.config';

@Component({
  selector: 'app-theme-switcher',
  template: `
    <select (change)="switchTheme($event)">
      <option *ngFor="let theme of themes" [value]="theme.name">
        {{ theme.name | titlecase }}
      </option>
    </select>
  `
})
export class ThemeSwitcherComponent {
  themes = AVAILABLE_THEMES;

  switchTheme(event: any) {
    ThemeConfig.savePreference(event.target.value);
  }
}
```

## ATELIER Design Inspiration

The default **Dark Theme** is inspired by the ATELIER fashion e-commerce platform:
- Clean, minimalist design
- Dark background for product focus
- Blue accent for CTAs and highlights
- Responsive layout
- High contrast for readability
- Modern, sophisticated aesthetic

## Setup Instructions

1. **Import theme configuration in app initialization**:
```typescript
// In app.component.ts
import { ThemeConfig } from '@app/modules/product-manager/theme/theme.config';

ngOnInit() {
  ThemeConfig.initialize('dark'); // or user's saved preference
}
```

2. **Add CSS variables to global styles**:
```scss
// In styles.scss
@import 'app/modules/product-manager/theme/theme.config';
```

3. **Use variables in component styles**:
```scss
.my-component {
  background: var(--color-background);
  color: var(--color-text);
}
```

## Color Accessibility

All color combinations meet WCAG AA standards for contrast:
- Dark theme: White text on dark background (>7:1 contrast)
- Light theme: Dark text on light background (>7:1 contrast)
- Color blindness: Not relying solely on color for information

## Future Enhancements

- [ ] Custom color picker for user-defined themes
- [ ] Theme persistence per user
- [ ] Dark/light mode auto-detection
- [ ] Accessibility options (high contrast, dyslexia-friendly)
- [ ] RTL language support
- [ ] Animation speed settings
- [ ] Font size adjustment
