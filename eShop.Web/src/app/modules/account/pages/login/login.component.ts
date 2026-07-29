import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AppAuthService } from '../../../../core/auth/app-auth.service';

@Component({
  selector: 'app-account-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginPageComponent implements OnInit, AfterViewInit {
  submitting = false;
  dark = false;
  showPassword = false;
  returnUrl: string = '';

  constructor(
    public authService: AppAuthService,
    private route: ActivatedRoute
  ) { }
  @ViewChild('username', { static: false }) usernameInput!: ElementRef<HTMLInputElement>;

  ngOnInit(): void {
    this.authService.clear();
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
  }
  ngAfterViewInit(): void {
    setTimeout(() => this.usernameInput?.nativeElement?.focus(), 0);  // Tập trung vào trường username sau khi view đã được khởi tạo 
  }
  login(): void {
    this.submitting = true;
    this.authService.authenticate(() => (this.submitting = false), this.returnUrl);
  }
  ToggleDarkMode(): void {
    this.dark = !this.dark;
  }
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
