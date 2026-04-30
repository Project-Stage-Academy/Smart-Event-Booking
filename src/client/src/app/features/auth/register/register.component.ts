import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

const passwordsMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;

  if (!password || !confirmPassword) {
    return null;
  }

  return password === confirmPassword ? null : { passwordMismatch: true };
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly formBuilder = inject(FormBuilder);

  isSubmitting = false;
  successMessage = '';
  errorMessages: string[] = [];

  readonly registerForm = this.formBuilder.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
  }, { validators: passwordsMatchValidator });

  submit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessages = [];

    const request = this.registerForm.getRawValue();
    const lastName = request.lastName.trim();

    this.authService.register({
      ...request,
      lastName: lastName.length > 0 ? lastName : undefined
    })
      .pipe(finalize(() => {
        this.isSubmitting = false;
      }))
      .subscribe({
        next: (response) => {
          this.successMessage = response.message;
          this.registerForm.reset();
          this.registerForm.patchValue({
            firstName: '',
            lastName: '',
            email: '',
            password: '',
            confirmPassword: ''
          });
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

    return ['Registration failed. Please review your details and try again.'];
  }
}
