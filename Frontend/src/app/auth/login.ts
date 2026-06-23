import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from './auth.service';

type Mode = 'login' | 'register';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly mode = signal<Mode>('login');
  readonly registerStep = signal<1 | 2>(1);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // Login fields.
  readonly email = signal('');
  readonly password = signal('');

  // Register fields (page 1).
  readonly regEmail = signal('');
  readonly firstName = signal('');
  readonly lastName = signal('');
  readonly age = signal<number | null>(null);
  readonly isGovernmentOfficial = signal(false);
  readonly isLawEnforcementOrMilitary = signal(false);

  // Register fields (page 2).
  readonly regPassword = signal('');
  readonly confirmPassword = signal('');

  // ---- Mode switching ---------------------------------------------------

  showRegister(): void {
    this.error.set(null);
    this.registerStep.set(1);
    this.mode.set('register');
  }

  showLogin(): void {
    this.error.set(null);
    this.mode.set('login');
  }

  // ---- Login ------------------------------------------------------------

  submitLogin(): void {
    if (!this.email().trim() || !this.password()) {
      this.error.set('Enter your email and password.');
      return;
    }
    this.run(this.auth.login({ email: this.email().trim(), password: this.password() }));
  }

  // ---- Register, two pages ----------------------------------------------

  registerNext(): void {
    if (!this.regEmail().trim()) { this.error.set('Email is required.'); return; }
    if (!this.firstName().trim() || !this.lastName().trim()) {
      this.error.set('First and last name are required.');
      return;
    }
    const age = this.age();
    if (age === null || age < 1 || age > 130) { this.error.set('Enter a valid age (1–130).'); return; }

    this.error.set(null);
    this.registerStep.set(2);
  }

  registerBack(): void {
    this.error.set(null);
    this.registerStep.set(1);
  }

  submitRegister(): void {
    if (this.regPassword().length < 6) {
      this.error.set('Password must be at least 6 characters.');
      return;
    }
    if (this.regPassword() !== this.confirmPassword()) {
      this.error.set('Passwords do not match.');
      return;
    }

    this.run(this.auth.register({
      email: this.regEmail().trim(),
      firstName: this.firstName().trim(),
      lastName: this.lastName().trim(),
      age: this.age()!,
      isGovernmentOfficial: this.isGovernmentOfficial(),
      isLawEnforcementOrMilitary: this.isLawEnforcementOrMilitary(),
      password: this.regPassword(),
    }));
  }

  // ---- Shared submit handling -------------------------------------------

  private run(request: import('rxjs').Observable<unknown>): void {
    this.loading.set(true);
    this.error.set(null);
    request.subscribe({
      next: () => {
        this.loading.set(false);
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/book';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.error.set(this.messageFor(err));
      },
    });
  }

  private messageFor(err: HttpErrorResponse): string {
    if (err.status === 0) return 'Could not reach the server. Is the API running on :5162?';
    // ASP.NET ProblemDetails carries a human-readable "detail".
    const detail = err.error?.detail ?? err.error?.title;
    if (typeof detail === 'string' && detail) return detail;
    if (err.status === 401) return 'Invalid email or password.';
    if (err.status === 409) return 'An account with that email already exists.';
    return 'Something went wrong. Please try again.';
  }
}
