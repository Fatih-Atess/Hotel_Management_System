import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

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
  }

  errorMessage: string = ''

  constructor(
    private authService: Auth,
    private router: Router
  ) { }

  onSubmit() {
    this.errorMessage = '';

    this.authService.login(this.loginData).subscribe({
      next: () => {
        this.router.navigate(['reservations']);
      },
      error: (err) => {
        console.error('Login error:', err);
        this.errorMessage = 'Giriş başarısız. Lütfen kullanıcı adı ve şifrenizi kontrol edin.';
      }
    });
  }

}
