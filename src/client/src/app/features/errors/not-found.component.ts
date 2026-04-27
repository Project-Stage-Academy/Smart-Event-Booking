import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="error-container">
      <h1 class="error-code not-found">404</h1>
      <p class="error-message">Oops! Page not found.</p>
      <p class="error-description">
        The page you’re looking for doesn’t exist or has been moved.
      </p>
      <a routerLink="/" class="btn-home">Go Home</a>
    </div>
  `,
  styleUrl: './errors.scss'
})
export class NotFoundComponent {}
