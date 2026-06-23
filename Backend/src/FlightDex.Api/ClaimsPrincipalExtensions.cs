using System.Security.Claims;

namespace FlightDex.Api;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Reads the authenticated user's id from the JWT "sub" claim.</summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        // TODO: parse the subject/NameIdentifier claim to an int.
        throw new NotImplementedException();
    }
}
