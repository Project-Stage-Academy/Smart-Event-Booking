import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

const passwordsMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;

  if (!password || !confirmPassword) {
    return null;
  }

  return password === confirmPassword ? null : { mismatch: true };
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly registerForm = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordsMatchValidator });

  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const request = this.registerForm.getRawValue();
    const lastName = request.lastName.trim();

    this.authService.register({
      ...request,
      lastName: lastName.length > 0 ? lastName : undefined
    })
      .pipe(finalize(() => {
        this.isLoading.set(false);
      }))
      .subscribe({
        next: () => {
          this.router.navigate(['/events'], { queryParams: { registered: true } });
        },
        error: (err: HttpErrorResponse) => {
          this.errorMessage.set(this.extractError(err));
        }
      });
  }

  private extractError(err: HttpErrorResponse): string {
    if (err.error && Array.isArray(err.error)) {
      return err.error.join(', ');
    } else if (err.error && typeof err.error === 'object' && err.error.errors) {
      return Object.values(err.error.errors).flat().join(', ');
    } else {
      return err.error?.message || err.message || 'An error occurred during registration.';
    }
  }
}
