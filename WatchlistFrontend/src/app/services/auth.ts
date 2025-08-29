import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

// Make sure this path is correct for your project structure
import { RegisterDto, LoginDto, AuthResponseDto } from '../models/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // This base URL will be proxied to your backend (e.g., https://localhost:7069/api/Auth)
  private readonly apiUrl = '/api/Auth';

  constructor(private http: HttpClient, private router: Router) { }


  public register(data: RegisterDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, data).pipe(
      tap(response => this.setSession(response))
    );
  }

  public login(data: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, data).pipe(
      tap(response => this.setSession(response))
    );
  }

  public logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.router.navigate(['/login']);
  }

  private setSession(authResponse: AuthResponseDto): void {
    localStorage.setItem('access_token', authResponse.accessToken);
    localStorage.setItem('refresh_token', authResponse.refreshToken);
  }

  public isLoggedIn(): boolean {
    return !!localStorage.getItem('access_token');
  }

  public getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  public getUserId(): string | null {
    const token = this.getAccessToken();
    if (!token) {
      return null;
    }

    try {
      // The user ID is in the 'sub' (subject) claim of the JWT payload.
      // We decode it from the base64url encoded string.
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub;
    } catch (e) {
      console.error('Error decoding token', e);
      return null;
    }
  }
}
