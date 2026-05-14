/**
 * Product Manager Theme & Color Palette
 * 
 * Dark theme with blue accent (inspired by ATELIER design)
 * Supports multiple color schemes: Dark, Light, Professional
 */

export interface ThemeColor {
  primary: string;
  secondary: string;
  background: string;
  surface: string;
  text: string;
  textSecondary: string;
  border: string;
  success: string;
  warning: string;
  error: string;
  info: string;
}

export interface Theme {
  name: string;
  colors: ThemeColor;
}

/**
 * Dark Theme (Default - ATELIER Style)
 * Luxury fashion brand aesthetic with champagne gold accents
 * Deep black background with elegant cream text
 */
export const DARK_THEME: Theme = {
  name: 'dark',
  colors: {
    primary: '#D4AF37',           // Champagne Gold (Accent)
    secondary: '#C5B382',         // Soft Gold
    background: '#121212',        // Deep black
    surface: '#1E1E1E',           // Dark charcoal
    text: '#F5F4F0',              // Cream white
    textSecondary: '#A3A3A3',     // Ash gray
    border: '#2A2A2A',            // Dark gray border
    success: '#2E7D32',           // Dark green
    warning: '#F57C00',           // Dark orange
    error: '#C62828',             // Dark red
    info: '#1565C0'               // Dark blue
  }
};

/**
 * Light Theme
 * Clean, professional look for daytime use
 */
export const LIGHT_THEME: Theme = {
  name: 'light',
  colors: {
    primary: '#0056b3',           // Dark Blue
    secondary: '#6c757d',         // Gray
    background: '#f5f5f5',        // Light Gray Background
    surface: '#ffffff',           // White cards
    text: '#212121',              // Dark text
    textSecondary: '#666666',     // Medium gray text
    border: '#e0e0e0',            // Light border
    success: '#28a745',           // Green
    warning: '#ffc107',           // Yellow
    error: '#dc3545',             // Red
    info: '#17a2b8'               // Cyan
  }
};

/**
 * Professional Theme
 * Corporate, sophisticated look
 */
export const PROFESSIONAL_THEME: Theme = {
  name: 'professional',
  colors: {
    primary: '#1e3a8a',           // Deep Blue
    secondary: '#64748b',         // Slate Gray
    background: '#f8fafc',        // Off-white
    surface: '#ffffff',           // White
    text: '#0f172a',              // Very dark blue
    textSecondary: '#475569',     // Slate
    border: '#cbd5e1',            // Light slate
    success: '#16a34a',           // Medium Green
    warning: '#d97706',           // Amber
    error: '#dc2626',             // Bright Red
    info: '#0284c7'               // Sky Blue
  }
};

/**
 * Modern Dark Theme
 * Contemporary design with vibrant accents
 */
export const MODERN_DARK_THEME: Theme = {
  name: 'modern-dark',
  colors: {
    primary: '#3b82f6',           // Blue-500
    secondary: '#8b5cf6',         // Purple
    background: '#0f172a',        // Very dark blue
    surface: '#1e293b',           // Dark slate
    text: '#f1f5f9',              // Almost white
    textSecondary: '#cbd5e1',     // Light slate
    border: '#334155',            // Slate
    success: '#10b981',           // Emerald
    warning: '#f59e0b',           // Amber
    error: '#ef4444',             // Red
    info: '#06b6d4'               // Cyan
  }
};

/**
 * All available themes
 */
export const AVAILABLE_THEMES: Theme[] = [
  DARK_THEME,
  LIGHT_THEME,
  PROFESSIONAL_THEME,
  MODERN_DARK_THEME
];

/**
 * Theme Configuration Service
 */
export class ThemeConfig {
  private static currentTheme: Theme = DARK_THEME;

  /**
   * Get current theme
   */
  static getTheme(): Theme {
    return this.currentTheme;
  }

  /**
   * Set theme by name
   */
  static setTheme(themeName: string): void {
    const theme = AVAILABLE_THEMES.find(t => t.name === themeName);
    if (theme) {
      this.currentTheme = theme;
      this.applyThemeToDOM(theme);
    }
  }

  /**
   * Apply theme colors to DOM
   */
  static applyThemeToDOM(theme: Theme): void {
    const root = document.documentElement;
    const colors = theme.colors;

    root.style.setProperty('--color-primary', colors.primary);
    root.style.setProperty('--color-secondary', colors.secondary);
    root.style.setProperty('--color-background', colors.background);
    root.style.setProperty('--color-surface', colors.surface);
    root.style.setProperty('--color-text', colors.text);
    root.style.setProperty('--color-text-secondary', colors.textSecondary);
    root.style.setProperty('--color-border', colors.border);
    root.style.setProperty('--color-success', colors.success);
    root.style.setProperty('--color-warning', colors.warning);
    root.style.setProperty('--color-error', colors.error);
    root.style.setProperty('--color-info', colors.info);
  }

  /**
   * Get color from current theme
   */
  static getColor(colorKey: keyof ThemeColor): string {
    return this.currentTheme.colors[colorKey];
  }

  /**
   * Initialize theme on app load
   */
  static initialize(themeName: string = 'dark'): void {
    const savedTheme = localStorage.getItem('selected-theme') || themeName;
    this.setTheme(savedTheme);
  }

  /**
   * Save user theme preference
   */
  static savePreference(themeName: string): void {
    localStorage.setItem('selected-theme', themeName);
    this.setTheme(themeName);
  }
}

/**
 * CSS Variables for themes
 * Add this to your global styles.scss:
 *
 * :root {
 *   --color-primary: #007bff;
 *   --color-secondary: #6c757d;
 *   --color-background: #1a1a1a;
 *   --color-surface: #2d2d2d;
 *   --color-text: #ffffff;
 *   --color-text-secondary: #b0b0b0;
 *   --color-border: #404040;
 *   --color-success: #28a745;
 *   --color-warning: #ffc107;
 *   --color-error: #dc3545;
 *   --color-info: #17a2b8;
 * }
 *
 * Example usage in component:
 * background-color: var(--color-primary);
 * color: var(--color-text);
 * border: 1px solid var(--color-border);
 */
