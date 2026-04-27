import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError, EMPTY } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error) {
        switch (error.status) {
          case 401:
            router.navigateByUrl('/auth/login');
            return EMPTY;
          case 403:
            router.navigateByUrl('/forbidden');
            return EMPTY;
          case 404:
            router.navigateByUrl('/not-found');
            return EMPTY;
          case 500:
            router.navigateByUrl('/server-error');
            return EMPTY;
        }
      }
      return throwError(() => error);
    })
  );
};
