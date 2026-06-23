using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Domain;
using Microsoft.Extensions.Options;

namespace FlightDex.Booking.Infrastructure.Security;

/// <summary>Issues signed JWTs from the configured <see cref="JwtOptions"/>.</summary>
internal sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public string CreateToken(User user)
    {
        // TODO: build claims (sub = user.Id), sign with the symmetric key, return the JWT string.
        throw new NotImplementedException();
    }
}
