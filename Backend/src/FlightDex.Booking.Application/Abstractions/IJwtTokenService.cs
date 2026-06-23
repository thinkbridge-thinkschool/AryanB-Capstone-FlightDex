using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Issues signed JWT access tokens for authenticated users.</summary>
public interface IJwtTokenService
{
    AccessToken Create(User user);
}

/// <summary>A signed bearer token and the instant it expires (UTC).</summary>
public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);
