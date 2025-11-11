import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, tap, catchError, map } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

interface LoginRequest {
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.API_URL + 'auth';
  private isRefreshing = false;

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  // LOGIN
  login(credentials: LoginRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/login`, credentials, {
      withCredentials: true
    }).pipe(
      tap(() => console.log('✅ Login successful — cookies set by backend'))
    );
  }

  // LOGOUT
  logout(): void {
    this.http.post(`${this.API_URL}/logout`, {}, { withCredentials: true }).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: () => {
        this.router.navigate(['/login']);
      }
    });
  }

  // TEST AUTH (verifica si el usuario está logueado haciendo ping a un endpoint protegido)
  isAuthenticated(): Observable<boolean> {
    return this.http.get(`${this.API_URL}/me`, {
      withCredentials: true
    }).pipe(
      map(() => true), // Si responde 200 → autenticado
      catchError(() => of(false))
    );
  }

  // REFRESH TOKEN
  refreshToken(): Observable<any> {
    if (this.isRefreshing) return of(null);
    this.isRefreshing = true;

    const csrfToken = this.getCsrfToken();

    // 👇 crear headers correctamente tipados
    let headers: HttpHeaders | undefined = undefined;
    if (csrfToken) {
      headers = new HttpHeaders({ 'X-XSRF-TOKEN': csrfToken });
    }

    return this.http.post(`${this.API_URL}/refresh`, {}, {
      withCredentials: true,
      headers
    }).pipe(
      tap(() => {
        this.isRefreshing = false;
        console.log('🔁 Tokens refreshed successfully');
      }),
      catchError(err => {
        this.isRefreshing = false;
        this.logout();
        throw err;
      })
    );
  }

  // Extraer CSRF token (solo esta cookie es accesible)
  private getCsrfToken(): string | null {
    const match = document.cookie.match(/csrf_token=([^;]+)/);
    return match ? decodeURIComponent(match[1]) : null;
  }
}
