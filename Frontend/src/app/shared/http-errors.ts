import { HttpErrorResponse } from '@angular/common/http';

/**
 * Turns an {@link HttpErrorResponse} into a user-facing message. Centralises the two
 * patterns every page used to hand-roll: the "server unreachable" (status 0) case and
 * ASP.NET ProblemDetails (`detail`/`title`) extraction.
 *
 * @param err      the error from a failed HttpClient call.
 * @param fallback message to show when the server returned no usable detail.
 */
export function httpErrorMessage(
  err: HttpErrorResponse,
  fallback = 'Something went wrong. Please try again.',
): string {
  if (err.status === 0) return 'Could not reach the server. Please try again later.';
  const detail = err.error?.detail ?? err.error?.title;
  return typeof detail === 'string' && detail ? detail : fallback;
}
