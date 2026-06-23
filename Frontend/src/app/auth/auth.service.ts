import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResult, AuthUser, LoginRequest, RegisterRequest } from './auth.models';

const STORAGE_KEY = 'flightdex.auth';

/**
 * Holds the authentication session: the bearer token and the signed-in user. The session
 * is persisted to localStorage so a page refresh keeps the user logged in until the token
 * expires.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly session = signal<AuthResult | null>(this.restore());

  /** The signed-in user, or null. Reactive — the titlebar reads this. */
  readonly user = computed<AuthUser | null>(() => this.session()?.user ?? null);
  readonly isAuthenticated = computed(() => this.session() !== null);

  login(request: LoginRequest): Observable<AuthResult> {
    return this.http.post<AuthResult>('/auth/login', request).pipe(tap(r => this.persist(r)));
  }

  register(request: RegisterRequest): Observable<AuthResult> {
    return this.http.post<AuthResult>('/auth/register', request).pipe(tap(r => this.persist(r)));
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.session.set(null);
  }

  /** The raw bearer token for the auth interceptor, or null if signed out/expired. */
  getToken(): string | null {
    const s = this.session();
    return s && !this.isExpired(s) ? s.token : null;
  }

  private persist(result: AuthResult): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(result));
    this.session.set(result);
  }

  private restore(): AuthResult | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try {
      const parsed = JSON.parse(raw) as AuthResult;
      if (this.isExpired(parsed)) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return parsed;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }

  private isExpired(result: AuthResult): boolean {
    const expiry = Date.parse(result.expiresAtUtc);
    return Number.isFinite(expiry) && expiry <= Date.now();
  }
}
