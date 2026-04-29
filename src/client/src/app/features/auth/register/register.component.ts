import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

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

  registerForm: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: this.passwordMatchValidator });

  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : { 'mismatch': true };
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set(null);

      this.authService.register(this.registerForm.value)
        .subscribe({
          next: (response: any) => {
            this.isLoading.set(false);
            
            if (response && response.succeeded === false) {
              const msg = Array.isArray(response.errors) 
                ? response.errors.join(', ') 
                : 'Registration failed.';
              this.errorMessage.set(msg);
              return;
            }

            this.router.navigate(['/events'], { queryParams: { registered: true } });
          },
          error: (err) => {
            this.isLoading.set(false);
            
            let msg = '';
            if (err.error && Array.isArray(err.error)) {
              msg = err.error.join(', ');
            } else if (err.error && typeof err.error === 'object' && err.error.errors) {
              msg = Object.values(err.error.errors).flat().join(', ');
            } else {
              msg = err.error?.message || err.message || 'An error occurred during registration.';
            }
            
            this.errorMessage.set(msg);
          }
        });
    }
  }
}
