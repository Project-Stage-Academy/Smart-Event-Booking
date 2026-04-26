import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error) {
        switch (error.status) {
          case 400:
            // Often handled by the component for validation errors,
            // but you could redirect to a general bad request page if needed.
            break;
          case 401:
            // Redirect to login or handle unauthorized
            router.navigateByUrl('/auth/login');
            break;
          case 403:
            router.navigateByUrl('/forbidden');
            break;
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            router.navigateByUrl('/server-error');
            break;
        }
      }
      return throwError(() => error);
    })
  );
};
