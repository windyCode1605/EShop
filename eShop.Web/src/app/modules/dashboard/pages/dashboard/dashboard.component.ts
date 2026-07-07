import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="max-w-[1280px] mx-auto px-4">
      <!-- Asymmetric Hero Section -->
      <section class="grid grid-cols-1 lg:grid-cols-2 gap-12 lg:gap-24 min-h-[80vh] items-center py-16 lg:py-24">
        <div class="max-w-xl">
          <span class="text-xs uppercase tracking-[0.2em] text-[#71717A] font-medium mb-6 block">EShop Premium</span>
          <h1 class="text-5xl lg:text-7xl font-semibold text-[#18181B] leading-[1.1] tracking-tight mb-8">
            The quiet luxury of seamless shopping.
          </h1>
          <p class="text-lg text-[#71717A] mb-10 leading-relaxed">
            Discover a curated collection of essentials, designed with intention and crafted without compromise. 
            Experience the new standard of digital commerce.
          </p>
          <button (click)="navigateToProducts()" class="inline-flex items-center justify-center bg-[#2563EB] text-white px-8 py-4 rounded-[14px] hover:bg-[#1D4ED8] transition-transform hover:-translate-y-0.5 font-medium">
            Explore Collection
          </button>
        </div>
        <div class="relative h-[60vh] lg:h-[75vh] w-full rounded-[24px] overflow-hidden bg-[#F4F4F5]">
          <img 
            src="https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&q=80&w=1000" 
            alt="Premium lifestyle"
            class="w-full h-full object-cover"
          />
        </div>
      </section>

      <!-- Featured Categories (Zigzag / Asymmetric) -->
      <section class="py-24 border-t border-[#E4E4E7]">
        <div class="max-w-2xl mb-20">
          <h2 class="text-3xl font-semibold text-[#18181B] mb-4 tracking-tight">Curated Selections</h2>
          <p class="text-lg text-[#71717A]">Our finest categories, handpicked for quality and design.</p>
        </div>

        <div class="space-y-32">
          <!-- Item 1 -->
          <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-20 items-center">
            <div class="lg:col-span-7 rounded-[24px] overflow-hidden aspect-[4/3] bg-[#F4F4F5] group">
              <img src="https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=800" alt="Audio" class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"/>
            </div>
            <div class="lg:col-span-5 p-4 lg:p-8">
              <span class="text-xs uppercase tracking-widest text-[#71717A] font-medium mb-4 block">01 // Acoustic</span>
              <h3 class="text-3xl font-semibold text-[#18181B] mb-6">High-Fidelity Audio</h3>
              <p class="text-[#71717A] mb-8 leading-relaxed">Experience sound in its purest form. Our acoustic collection is engineered for audiophiles who demand nothing but perfection.</p>
              <a href="#" class="inline-flex items-center text-[#18181B] font-medium border-b border-[#18181B] pb-1 hover:text-[#71717A] hover:border-[#71717A] transition-colors">
                Discover Audio →
              </a>
            </div>
          </div>

          <!-- Item 2 -->
          <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-20 items-center">
            <div class="lg:col-span-5 order-2 lg:order-1 p-4 lg:p-8">
              <span class="text-xs uppercase tracking-widest text-[#71717A] font-medium mb-4 block">02 // Timepieces</span>
              <h3 class="text-3xl font-semibold text-[#18181B] mb-6">Precision Wearables</h3>
              <p class="text-[#71717A] mb-8 leading-relaxed">Master time with our exclusive range of smart and analog timepieces. Crafted for those who value every second.</p>
              <a href="#" class="inline-flex items-center text-[#18181B] font-medium border-b border-[#18181B] pb-1 hover:text-[#71717A] hover:border-[#71717A] transition-colors">
                Discover Wearables →
              </a>
            </div>
            <div class="lg:col-span-7 order-1 lg:order-2 rounded-[24px] overflow-hidden aspect-[4/3] bg-[#F4F4F5] group">
              <img src="https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800" alt="Watches" class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"/>
            </div>
          </div>
        </div>
      </section>
    </div>
  `
})
export class DashboardComponent {
  navigateToProducts() {
    window.location.href = '/product';
  }
}
