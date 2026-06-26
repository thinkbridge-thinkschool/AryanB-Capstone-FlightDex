import { ApplicationConfig, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { authInterceptor } from './auth/auth.interceptor';
import { apiBaseUrlInterceptor } from './core/api-base-url.interceptor';
import { loadRuntimeConfig } from './core/runtime-config';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    // apiBaseUrlInterceptor first (rewrites the URL for cross-origin deploys), then the bearer token.
    provideHttpClient(withFetch(), withInterceptors([apiBaseUrlInterceptor, authInterceptor])),
    // Load /config.json (the API base URL) before the app renders.
    provideAppInitializer(loadRuntimeConfig),
  ]
};
