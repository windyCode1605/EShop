import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AppAuthService } from '../../../shared/auth/app-auth.service';

@Component({
  selector: 'app-account-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginPageComponent implements OnInit , AfterViewInit{
  submitting = false;
  dark = false;
  constructor( 
    public authService: AppAuthService,
  ) { }
  @ViewChild('username', { static: false}) usernameInput!: ElementRef<HTMLInputElement>;

  ngOnInit(): void {
    this.authService.clear();
  }
  ngAfterViewInit(): void {
      setTimeout(() => this.usernameInput?.nativeElement?.focus(), 0);  // Tập trung vào trường username sau khi view đã được khởi tạo 
  }
  login(): void {
    console.log('Đang đăng nhập...');
    console.log('Thông tin đăng nhập:', this.authService.authenticateModel);
    this.submitting = true;
    this.authService.authenticate(() => (this.submitting = false));
  }
  ToggleDarkMode(): void {
    this.dark = !this.dark;
  }
}
