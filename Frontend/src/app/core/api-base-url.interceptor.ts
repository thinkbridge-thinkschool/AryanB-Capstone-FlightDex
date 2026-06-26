import { HttpInterceptorFn } from '@angular/common/http';
import { getApiBaseUrl } from './runtime-config';

// The relative API roots the services call. Scoped so /config.json and static assets are untouched.
const API_PREFIXES = ['/flight', '/airports', '/auth', '/ticket'];

/**
 * Prepends the configured API base URL to API requests. A no-op when apiBaseUrl is empty
 * (local dev / same-origin), so the services keep using their relative URLs unchanged.
 */
export const apiBaseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  const base = getApiBaseUrl();
  if (base && req.url.startsWith('/') && API_PREFIXES.some((p) => req.url.startsWith(p))) {
    return next(req.clone({ url: base + req.url }));
  }
  return next(req);
};
