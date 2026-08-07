import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CartService } from '../../../cart/services/cart.service';
import { Router } from '@angular/router';
import { AddressService } from '../../services/address.service';
import { Address } from '../../models/address.model';
import { OrderService, CreateOrderRequest } from '../../../order/services/order.service';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout-page.component.html',
  styleUrls: ['./checkout-page.component.scss']
})
export class CheckoutPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private router = inject(Router);

  constructor(
    private addressService: AddressService,
    private orderService: OrderService
  ) { }

  currentStep = signal<1 | 2>(1);
  cartSummary = signal<any>(null);
  isLoading = signal(false);
  isSuccessModalOpen = signal(false);
  successOrderId = signal<number | null>(null);

  // State Địa chỉ - lấy trực tiếp từ Signal của Service
  savedAddresses = this.addressService.savedAddresses;
  addressLoading$ = this.addressService.loading$;

  selectedAddressId = signal<number | null>(null);
  isAddingNewAddress = signal<boolean>(false);

  // State Vận chuyển & Thanh toán
  shippingFee = signal<number>(0);

  // Tính tổng cộng
  estimatedTotal = computed(() => {
    const subtotal = this.cartSummary()?.subtotal || 0;
    return subtotal + this.shippingFee();
  });

  // Form nhập địa chỉ mới
  addressForm = this.fb.group({
    receiverName: ['', Validators.required],
    receiverPhone: ['', [Validators.required, Validators.pattern('^[0-9]{10,11}$')]],
    street: ['', Validators.required],
    city: ['', Validators.required],
    province: ['', Validators.required]
  });

  // Form Payment
  paymentForm = this.fb.group({
    paymentMethod: ['COD', Validators.required],
    shippingProvider: ['GHN'],
    note: ['']
  });

  ngOnInit() {
    this.cartService.getMyCart().subscribe(res => {
      if (res.success && res.data) {
        this.cartSummary.set(res.data);
      }
    });

    this.addressService.loadAddress().subscribe({
      next: () => {
        const addresses = this.savedAddresses();
        if (addresses.length > 0) {
          const defaultAddr = addresses.find((a: Address) => a.isDefault) || addresses[0];
          this.selectAddress(defaultAddr.id);
        } else {
          this.isAddingNewAddress.set(true);
        }
      },
      error: () => {
        // Nếu lỗi load, mở form thêm địa chỉ mới để user vẫn tiếp tục được
        this.isAddingNewAddress.set(true);
      }
    });
  }

  selectAddress(id: number) {
    this.selectedAddressId.set(id);
    this.isAddingNewAddress.set(false);
    this.calculateShipping();
  }

  toggleAddNewAddress() {
    this.isAddingNewAddress.set(true);
    this.selectedAddressId.set(null);
    this.shippingFee.set(0);
    this.addressForm.reset();
  }

  calculateShipping() {
    let province = '';

    if (this.isAddingNewAddress()) {
      province = this.addressForm.get('province')?.value || '';
    } else {
      const selectedId = this.selectedAddressId();
      const addr = this.savedAddresses().find((a: Address) => a.id === selectedId);
      if (addr) {
        province = addr.province;
      }
    }

    if (!province) {
      this.shippingFee.set(0);
      return;
    }

    const pName = province.toLowerCase();
    if (pName.includes('hồ chí minh') || pName.includes('hcm')) {
      this.shippingFee.set(20000);
    } else if (pName.includes('hà nội') || pName.includes('hn')) {
      this.shippingFee.set(30000);
    } else {
      this.shippingFee.set(40000);
    }
  }

  onAddressFormChange() {
    // Tính phí ngay khi nhập tỉnh, không cần form valid hoàn toàn
    this.calculateShipping();
  }

  // Wizard Navigation
  nextStep() {
    if (this.isAddingNewAddress()) {
      if (this.addressForm.invalid) {
        this.addressForm.markAllAsTouched();
        return;
      }

      const newAddr = this.addressForm.value as any;

      this.addressService.createNewAddress(newAddr).subscribe({
        next: (res) => {
          if (res.success && res.data) {
            this.selectAddress(res.data);
            if (this.shippingFee() === 0) {
              alert('Vui lòng nhập Tỉnh/Quận hợp lệ để tính phí giao hàng!');
              return;
            }
            this.currentStep.set(2);
          } else {
            alert('Có lỗi xảy ra khi tạo địa chỉ mới.');
          }
        },
        error: (err) => {
          alert('Lỗi khi lưu địa chỉ: ' + (err.message || 'Unknown error'));
        }
      });
    } else {
      if (!this.selectedAddressId()) {
        alert('Vui lòng chọn địa chỉ giao hàng!');
        return;
      }

      if (this.shippingFee() === 0) {
        alert('Vui lòng nhập Tỉnh/Quận hợp lệ để tính phí giao hàng!');
        return;
      }

      this.currentStep.set(2);
    }
  }

  prevStep() {
    this.currentStep.set(1);
  }

  placeOrder() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);

    const selectedId = this.selectedAddressId();
    const savedAddr = this.savedAddresses().find((a: Address) => a.id === selectedId);

    if (!savedAddr) {
      alert('Vui lòng chọn địa chỉ hợp lệ.');
      this.isLoading.set(false);
      return;
    }

    const payload: CreateOrderRequest = {
      addressId: selectedId || 0,
      receiverName: savedAddr.receiverName,
      receiverPhone: savedAddr.receiverPhone,
      street: savedAddr.street,
      city: savedAddr.city,
      province: savedAddr.province,
      paymentMethod: this.paymentForm.get('paymentMethod')?.value || 'COD',
      shippingProvider: this.paymentForm.get('shippingProvider')?.value || 'GHN',
      couponCode: '', // Currently no coupon input in UI, set empty
      note: this.paymentForm.get('note')?.value || ''
    };

    console.log('--- CALL API CREATE ORDER ---', payload);

    this.orderService.createOrder(payload).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.isSuccess || res.success) {
          const orderId = res.value?.id || res.data?.id;
          if (orderId) {
            this.successOrderId.set(orderId);
            this.isSuccessModalOpen.set(true);
          }
        } else {
          const errorMsg = res.message || (res.errors && res.errors.length > 0 ? res.errors[0] : 'Lỗi không xác định');
          alert('Có lỗi xảy ra: ' + errorMsg);
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        console.error(err);
        alert('Lỗi kết nối khi gọi API tạo đơn hàng!');
      }
    });
  }

  goToOrder() {
    const id = this.successOrderId();
    if (id) {
      this.router.navigate(['/order', id]);
    }
  }

  continueShopping() {
    this.router.navigate(['/product']);
  }
}
