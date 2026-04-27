import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="error-container">
      <h1 class="error-code forbidden">403</h1>
      <p class="error-message">Access Denied!</p>
      <p class="error-description">
        You do not have permission to access this resource.
      </p>
      <a routerLink="/" class="btn-home">Go Home</a>
    </div>
  `,
  styleUrl: './errors.scss'
})
export class ForbiddenComponent {}
