import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { Api } from '../../services/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  loginData = {
    username: '',
    password: ''
  };

  errorMessage: string = '';
  successMessage: string = '';
  isLoading: boolean = false;
  showPassword: boolean = false;
  isRegisterMode: boolean = false;

  constructor(
    private authService: Auth,
    private router: Router,
    private apiService: Api,
    private cdr: ChangeDetectorRef
  ) { }

  toggleMode() {
    this.isRegisterMode = !this.isRegisterMode;
    this.errorMessage = '';
    this.successMessage = '';
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  onSubmit() {
    if (!this.loginData.username || !this.loginData.password) return;

    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;

    if (this.isRegisterMode) {
      this.apiService.addUser({
        kullanici_Adi: this.loginData.username,
        sifre: this.loginData.password
      }).subscribe({
        next: () => {
          this.isLoading = false;
          this.successMessage = 'Kayıt başarılı şimdi giriş yapabilirsiniz';
          this.isRegisterMode = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Kayıt hatası:', err);
          if (err.status === 409 || typeof err.error === 'string') {
            this.errorMessage = err.error || 'Bu kullanıc adı zaten alınmış';
          } else {
            this.errorMessage = 'Kayıt yapılırken bir hata oluştu.';
          }
          this.cdr.detectChanges();
        }
      });
    } else {

      this.authService.login(this.loginData).subscribe({
        next: (res: any) => {
          this.isLoading = false;
          const role = (res?.role || this.authService.getRole() || '').toLowerCase();
          if (role === 'admin') {
            this.router.navigate(['/admin']);
          } else {
            this.router.navigate(['/customer']);
          }
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Login error:', err);
          this.errorMessage = 'Giriş başarısız. Lütfen kullanıcı adı ve şifrenizi kontrol edin.';
          this.cdr.detectChanges();
        }
      });
    }
  }

}
