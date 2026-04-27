import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="error-container">
      <h1 class="error-code server-error">Error</h1>
      <p class="error-message">Oops! Something went wrong.</p>
      <p class="error-description">
        An unexpected error occurred while processing your request.
      </p>
      <a routerLink="/" class="btn-home">Go Home</a>
    </div>
  `,
  styleUrl: './errors.scss'
})
export class ServerErrorComponent {}
