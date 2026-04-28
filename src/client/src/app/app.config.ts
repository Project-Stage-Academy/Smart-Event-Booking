import { ApplicationConfig, inject, provideBrowserGlobalErrorListeners, provideAppInitializer } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { ConfigService } from './core/services/config.service';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    
    provideHttpClient(withInterceptors([errorInterceptor])),
    
    provideAppInitializer(() => {
      const configService = inject(ConfigService);
      return configService.load();
    })
  ]
};
