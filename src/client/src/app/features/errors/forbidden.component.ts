import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="error-container">
      <h1 class="error-code">403</h1>
      <p class="error-message">Access Denied!</p>
      <p class="error-description">
        You do not have permission to access this resource.
      </p>
      <a routerLink="/" class="btn-home">Go Home</a>
    </div>
  `,
  styles: [`
    .error-container {
      text-align: center;
      padding: 100px 20px;
      font-family: sans-serif;
    }
    .error-code {
      font-size: 120px;
      font-weight: bold;
      color: #dc3545;
      margin: 0;
    }
    .error-message {
      font-size: 24px;
      margin: 10px 0;
    }
    .error-description {
      font-size: 18px;
      color: #6c757d;
      margin-bottom: 30px;
    }
    .btn-home {
      display: inline-block;
      padding: 10px 20px;
      background-color: #007bff;
      color: white;
      text-decoration: none;
      border-radius: 5px;
      font-weight: bold;
    }
    .btn-home:hover {
      background-color: #0056b3;
    }
  `]
})
export class ForbiddenComponent {}
