using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Issues signed JWT bearer tokens for authenticated users.</summary>
public interface IJwtTokenService
{
    string CreateToken(User user);
}
