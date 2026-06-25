using System.Net;
using System.Net.Http.Json;
using FlightDex.IntegrationTests.Infrastructure;

namespace FlightDex.IntegrationTests;

/// <summary>
/// API-level integration for the auth surface: registration validation, duplicate-email
/// conflict, and login success/failure — through the real controller + handlers + EF.
/// </summary>
[Collection("api")]
public sealed class AuthApiTests(FlightDexApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private static object Register(string email, int age = 30, string password = "Sup3rSecret!") => new
    {
        email,
        firstName = "Grace",
        lastName = "Hopper",
        age,
        isGovernmentOfficial = false,
        isLawEnforcementOrMilitary = false,
        password,
    };

    [Fact]
    public async Task Register_with_a_blank_password_is_rejected_with_400()
    {
        var response = await _client.PostAsJsonAsync("/auth/register", Register($"{Guid.NewGuid():N}@x.com", password: ""));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_with_an_out_of_range_age_is_rejected_with_400()
    {
        var response = await _client.PostAsJsonAsync("/auth/register", Register($"{Guid.NewGuid():N}@x.com", age: 0));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Registering_the_same_email_twice_returns_409_conflict()
    {
        var email = $"dupe-{Guid.NewGuid():N}@x.com";
        (await _client.PostAsJsonAsync("/auth/register", Register(email))).EnsureSuccessStatusCode();

        var second = await _client.PostAsJsonAsync("/auth/register", Register(email));
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Login_with_correct_credentials_returns_a_token()
    {
        var email = $"login-{Guid.NewGuid():N}@x.com";
        (await _client.PostAsJsonAsync("/auth/register", Register(email))).EnsureSuccessStatusCode();

        var login = await _client.PostAsJsonAsync("/auth/login", new { email, password = "Sup3rSecret!" });
        login.EnsureSuccessStatusCode();
        var auth = await login.ReadAsync<AuthResponse>();

        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
        Assert.Equal(email, auth.User.Email);
    }

    [Fact]
    public async Task Login_with_a_wrong_password_returns_401()
    {
        var email = $"wrongpw-{Guid.NewGuid():N}@x.com";
        (await _client.PostAsJsonAsync("/auth/register", Register(email))).EnsureSuccessStatusCode();

        var login = await _client.PostAsJsonAsync("/auth/login", new { email, password = "incorrect" });
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }
}
