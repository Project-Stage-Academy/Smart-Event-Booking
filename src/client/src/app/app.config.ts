import { ApplicationConfig, inject, provideBrowserGlobalErrorListeners, provideAppInitializer } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HttpInterceptorFn, provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { ConfigService } from './core/services/config.service';
import { errorInterceptor } from './core/interceptors/error.interceptor';

const withCredentialsInterceptor: HttpInterceptorFn = (req, next) =>
  next(req.clone({ withCredentials: true }));

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    
    provideHttpClient(withInterceptors([withCredentialsInterceptor, errorInterceptor])),
    
    provideAppInitializer(() => {
      const configService = inject(ConfigService);
      return configService.load();
    })
  ]
};
