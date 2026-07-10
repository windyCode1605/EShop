import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule],
  styleUrl: './admin-orders.component.scss',
  templateUrl: './admin-orders.component.html'
})
export class AdminOrdersComponent {
  isModalOpen = false;
  selectedOrderId = '';
  selectedStatus = '';

  openModal(orderId: string, currentStatus: string) {
    this.selectedOrderId = orderId;
    this.selectedStatus = currentStatus;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  onStatusChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedStatus = selectElement.value;
  }

  saveStatus() {
    // API logic to update status would go here
    this.closeModal();
  }
}
