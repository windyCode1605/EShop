import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CartService } from '../../../Cart/services/cart.service';
import { Router } from '@angular/router';
import { AddressService } from '../../services/Address.service';


interface Address {
  id: number;
  receiverName: string;
  receiverPhone: string;
  street: string;
  city: string;
  province: string;
  isDefault: boolean;
}

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout-page.component.html'
})
export class CheckoutPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private router = inject(Router);

  constructor(
    private addressService: AddressService
  ) { }


  currentStep = signal<1 | 2>(1); // Quản lý luồng Wizard
  cartSummary = signal<any>(null);
  isLoading = signal(false);

  // State Địa chỉ
  savedAddresses = this.addressService.savedAddresses;

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
      if (res.isSuccess) {
        this.cartSummary.set(res.value);
      }
    });

    this.addressService.loadAddress().subscribe(() => {
      const addresses = this.savedAddresses();
      if (addresses.length > 0) {
        const defaultAddr = addresses.find(a => a.isDefault) || addresses[0];
        this.selectAddress(defaultAddr.id);
      } else {
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
  }

  calculateShipping() {
    let province = '';

    if (this.isAddingNewAddress()) {
      if (this.addressForm.valid) {
        province = this.addressForm.get('province')?.value || '';
      }
    } else {
      const selectedId = this.selectedAddressId();
      const addr = this.savedAddresses().find(a => a.id === selectedId);
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
    if (this.addressForm.valid) {
      this.calculateShipping();
    }
  }

  // Wizard Navigation
  nextStep() {
    // Validate Step 1
    if (this.isAddingNewAddress() && this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      alert('Vui lòng điền đầy đủ thông tin địa chỉ mới!');
      return;
    }

    if (!this.isAddingNewAddress() && !this.selectedAddressId()) {
      alert('Vui lòng chọn địa chỉ giao hàng!');
      return;
    }

    if (this.shippingFee() === 0) {
      alert('Vui lòng nhập Tỉnh/Quận hợp lệ để tính phí giao hàng!');
      return;
    }

    this.currentStep.set(2); // Chuyển sang Bước 2
  }

  prevStep() {
    this.currentStep.set(1); // Quay lại Bước 1
  }

  placeOrder() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);

    const payload: any = {
      paymentMethod: this.paymentForm.get('paymentMethod')?.value,
      shippingProvider: this.paymentForm.get('shippingProvider')?.value,
      note: this.paymentForm.get('note')?.value
    };

    if (this.isAddingNewAddress()) {
      Object.assign(payload, this.addressForm.value);
    } else {
      payload.addressId = this.selectedAddressId();
    }

    console.log('--- MOCK CALL API CREATE ORDER ---');
    console.log('Payload:', payload);

    setTimeout(() => {
      this.isLoading.set(false);
      alert(`Đặt hàng thành công!\nPayload: ${JSON.stringify(payload)}`);
      // this.router.navigate(['/checkout/success']);
    }, 1500);
  }
}

