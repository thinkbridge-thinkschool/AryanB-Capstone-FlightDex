using System.IdentityModel.Tokens.Jwt;
using FlightDex.Booking.Domain;
using FlightDex.Booking.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace FlightDex.UnitTests.Booking;

public sealed class JwtTokenServiceTests
{
    private static readonly JwtOptions Options = new()
    {
        Issuer = "FlightDex",
        Audience = "FlightDexClient",
        Key = "unit-test-signing-key-at-least-32-bytes-long-0123456789",
        ExpiryMinutes = 60,
    };

    private static User SampleUser() => new(
        "jane@example.com", "Jane", "Doe", 30, false, false, "hash", "salt");

    [Fact]
    public void Create_issues_a_token_carrying_the_user_id_email_and_names()
    {
        var service = new JwtTokenService(MicrosoftOptions(Options));

        var token = service.Create(SampleUser());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Token);

        Assert.Equal("FlightDex", jwt.Issuer);
        Assert.Contains(jwt.Audiences, a => a == "FlightDexClient");
        Assert.Equal("jane@example.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Jane", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.GivenName).Value);
        Assert.Equal("Doe", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.FamilyName).Value);
        Assert.NotEmpty(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
    }

    [Fact]
    public void Create_sets_an_expiry_consistent_with_the_configured_lifetime()
    {
        var service = new JwtTokenService(MicrosoftOptions(Options));

        var token = service.Create(SampleUser());

        // ~60 minutes out; allow generous slack for clock + test execution time.
        Assert.True(token.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(58));
        Assert.True(token.ExpiresAtUtc < DateTime.UtcNow.AddMinutes(62));
    }

    private static IOptions<JwtOptions> MicrosoftOptions(JwtOptions o) => Microsoft.Extensions.Options.Options.Create(o);
}
