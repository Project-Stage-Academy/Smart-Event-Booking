import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);

  isSubmitting = false;
  successMessage = '';
  errorMessages: string[] = [];

  readonly loginForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    rememberMe: [false, Validators.required]
  });

  submit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessages = [];

    this.authService.login(this.loginForm.getRawValue())
      .pipe(finalize(() => {
        this.isSubmitting = false;
      }))
      .subscribe({
        next: (response) => {
          this.successMessage = response.message;
          this.loginForm.patchValue({ password: '' });
          void this.router.navigateByUrl('/');
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessages = this.extractErrors(error);
        }
      });
  }

  private extractErrors(error: HttpErrorResponse): string[] {
    if (!error.error) {
      return ['An unexpected error occurred.'];
    }

    if (Array.isArray(error.error)) {
      return error.error.map(item => String(item));
    }

    if (typeof error.error === 'string') {
      return [error.error];
    }

    return ['Login failed. Please check your credentials and try again.'];
  }
}
