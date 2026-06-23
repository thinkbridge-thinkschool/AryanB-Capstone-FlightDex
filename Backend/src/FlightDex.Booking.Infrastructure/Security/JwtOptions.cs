namespace FlightDex.Booking.Infrastructure.Security;

/// <summary>
/// JWT signing/validation settings, bound from the "Jwt" configuration section. The API
/// reads the same section to configure bearer-token validation.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "FlightDex";
    public string Audience { get; set; } = "FlightDexClient";

    /// <summary>HMAC signing key. Must be at least 32 bytes for HS256.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 1440;
}
