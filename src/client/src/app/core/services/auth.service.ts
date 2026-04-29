import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResponse, AuthSession } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private static readonly AUTH_SESSION_STORAGE_KEY = 'auth.session';

  private readonly http = inject(HttpClient);
  private readonly authSessionSignal = signal<AuthSession | null>(this.readStoredAuthSession());

  readonly authSession = computed(() => this.authSessionSignal());
  readonly userEmail = computed(() => this.authSessionSignal()?.email ?? null);
  readonly isLoggedIn = computed(() => this.authSessionSignal() !== null);

  private readonly authApiUrl = environment.apiUrl.endsWith('/events')
    ? environment.apiUrl.replace('/events', '/auth')
    : `${environment.apiUrl}/auth`;

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.authApiUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.authApiUrl}/login`, request)
      .pipe(tap((response) => {
        this.setAuthSession({
          email: request.email,
          rememberMe: request.rememberMe,
          message: response.message,
          loggedInAt: new Date().toISOString()
        });
      }));
  }

  clearAuthSession(): void {
    this.authSessionSignal.set(null);
    this.removeStoredAuthSession();
  }

  private setAuthSession(session: AuthSession): void {
    const normalizedEmail = session.email.trim();

    if (normalizedEmail.length === 0) {
      this.clearAuthSession();
      return;
    }

    const normalizedSession: AuthSession = {
      ...session,
      email: normalizedEmail
    };

    this.authSessionSignal.set(normalizedSession);
    this.storeAuthSession(normalizedSession);
  }

  private readStoredAuthSession(): AuthSession | null {
    if (typeof localStorage === 'undefined' || typeof sessionStorage === 'undefined') {
      return null;
    }

    const sessionValue = sessionStorage.getItem(AuthService.AUTH_SESSION_STORAGE_KEY);
    const localValue = localStorage.getItem(AuthService.AUTH_SESSION_STORAGE_KEY);

    return this.parseStoredAuthSession(sessionValue) ?? this.parseStoredAuthSession(localValue);
  }

  private storeAuthSession(session: AuthSession): void {
    if (typeof localStorage === 'undefined' || typeof sessionStorage === 'undefined') {
      return;
    }

    const serialized = JSON.stringify(session);

    if (session.rememberMe) {
      localStorage.setItem(AuthService.AUTH_SESSION_STORAGE_KEY, serialized);
      sessionStorage.removeItem(AuthService.AUTH_SESSION_STORAGE_KEY);
      return;
    }

    sessionStorage.setItem(AuthService.AUTH_SESSION_STORAGE_KEY, serialized);
    localStorage.removeItem(AuthService.AUTH_SESSION_STORAGE_KEY);
  }

  private removeStoredAuthSession(): void {
    if (typeof localStorage === 'undefined' || typeof sessionStorage === 'undefined') {
      return;
    }

    localStorage.removeItem(AuthService.AUTH_SESSION_STORAGE_KEY);
    sessionStorage.removeItem(AuthService.AUTH_SESSION_STORAGE_KEY);
  }

  private parseStoredAuthSession(rawValue: string | null): AuthSession | null {
    if (!rawValue) {
      return null;
    }

    try {
      const parsed = JSON.parse(rawValue) as Partial<AuthSession>;

      if (
        typeof parsed.email !== 'string' ||
        parsed.email.trim().length === 0 ||
        typeof parsed.rememberMe !== 'boolean' ||
        typeof parsed.message !== 'string' ||
        typeof parsed.loggedInAt !== 'string'
      ) {
        return null;
      }

      return {
        email: parsed.email.trim(),
        rememberMe: parsed.rememberMe,
        message: parsed.message,
        loggedInAt: parsed.loggedInAt
      };
    } catch {
      return null;
    }
  }
}
