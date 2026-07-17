import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Trigger recompile
@Component({
  selector: 'app-review-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './review-modal.component.html',
  styleUrls: ['./review-modal.component.scss']
})
export class ReviewModalComponent {
  @Input() product: any;
  @Output() close = new EventEmitter<void>();

  rating = signal<number>(0);
  comment = signal<string>('');
  isSubmitting = signal<boolean>(false);
  isSuccess = signal<boolean>(false);

  setRating(star: number) {
    this.rating.set(star);
  }

  submitReview() {
    if (this.rating() === 0) {
      alert('Vui lòng chọn số sao để đánh giá!');
      return;
    }

    this.isSubmitting.set(true);

    // MOCK API CALL
    console.log('=== SUBMITTING REVIEW ===');
    console.log('Product:', this.product);
    console.log('Rating:', this.rating());
    console.log('Comment:', this.comment());

    // Fake delay
    setTimeout(() => {
      this.isSubmitting.set(false);
      this.isSuccess.set(true);
      
      // Auto close after success
      setTimeout(() => {
        this.close.emit();
      }, 1500);
    }, 1000);
  }
}
