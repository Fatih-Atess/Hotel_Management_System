import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Auth {

  private apiUrl = 'http://localhost:5151/api/auth';

  constructor(private http: HttpClient) { }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('jwt_token', response.token);
        }
        if (response && response.role) {
          localStorage.setItem('user_role', response.role);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_role');
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  getRole(): string | null {
    return localStorage.getItem('user_role');
  }

  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  isAdmin(): boolean {
    const role = this.getRole();
    return role ? role.toLowerCase() === 'admin' : false;
  }

  isCustomer(): boolean {
    const role = this.getRole();
    if (!role) return false;
    const lower = role.toLowerCase();
    return lower === 'customer';
  }

}
