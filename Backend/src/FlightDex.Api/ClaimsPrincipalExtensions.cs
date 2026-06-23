using System.Security.Claims;

namespace FlightDex.Api;

internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// The authenticated user's id, read from the "sub" claim (mapped to NameIdentifier).
    /// Throws if absent — only call this on [Authorize]d actions where it must exist.
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var raw = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        return int.TryParse(raw, out var id)
            ? id
            : throw new InvalidOperationException("The token has no usable user id claim.");
    }
}
