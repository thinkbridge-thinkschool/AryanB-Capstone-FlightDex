import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

/**
 * Runtime configuration the SPA fetches at startup from /config.json (served as a static asset).
 *
 * Locally (dev `ng serve` and the docker-compose nginx build) apiBaseUrl is empty, so the
 * services' relative URLs (/flight, /auth, ...) are used as-is and proxied same-origin. In Azure
 * the file is rewritten at deploy time (azd web predeploy hook) to the API Container App URL, and
 * the apiBaseUrlInterceptor prepends it so the Static Web App can call the API cross-origin.
 */
export interface RuntimeConfig {
  apiBaseUrl: string;
}

let config: RuntimeConfig = { apiBaseUrl: '' };

/** The configured API base URL (no trailing slash), or '' to use relative same-origin URLs. */
export function getApiBaseUrl(): string {
  return config.apiBaseUrl;
}

/** APP_INITIALIZER: loads /config.json before the app renders. Falls back to relative URLs. */
export async function loadRuntimeConfig(): Promise<void> {
  const http = inject(HttpClient);
  try {
    const loaded = await firstValueFrom(http.get<RuntimeConfig>('/config.json'));
    if (loaded?.apiBaseUrl) {
      config = { apiBaseUrl: loaded.apiBaseUrl.replace(/\/+$/, '') };
    }
  } catch {
    // No config.json (or unreachable) — keep relative URLs.
  }
}
