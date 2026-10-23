
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface RegisterDto {
  fullName: string;
  email: string;
  mobile: string;
  password: string;
}
export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  success: boolean;
  message: string;
  data?: any;
}
export interface RegisterResponse {
  success: boolean;
  message: string;
  data?: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Auth`;

 register(data: RegisterDto): Observable<RegisterResponse> {
  return this.http.post<RegisterResponse>(
    `${this.apiUrl}/register`,
    data
  );
}

  login(data: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.apiUrl}/login`,
      data
    ).pipe(
      tap(response => this.saveTokens(response))
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();

    return this.http.post<AuthResponse>(
      `${this.apiUrl}/refresh`,
      { refreshToken }
    ).pipe(
      tap(response => this.saveTokens(response))
    );
  }

  logout(): Observable<unknown> {
    return this.http.post(
      `${this.apiUrl}/logout`,
      { refreshToken: this.getRefreshToken() }
    );
  }

  saveTokens(response: AuthResponse): void {
    sessionStorage.setItem(
      'accessToken',
      response.accessToken
    );

    sessionStorage.setItem(
      'refreshToken',
      response.refreshToken
    );
  }

  getToken(): string | null {
    return sessionStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem('refreshToken');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  clearSession(): void {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('refreshToken');
  }
}
