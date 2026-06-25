using FlightDex.Booking.Application;
using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Commands.Login;
using FlightDex.Booking.Domain;
using NSubstitute;

namespace FlightDex.UnitTests.Booking;

public sealed class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _tokens = Substitute.For<IJwtTokenService>();

    private LoginCommandHandler Sut() => new(_users, _hasher, _tokens);

    private static User Existing() => new(
        "jane@example.com", "Jane", "Doe", 30, false, false, "HASH", "SALT");

    [Fact]
    public async Task Throws_invalid_credentials_when_the_email_is_unknown()
    {
        _users.GetByEmailAsync("ghost@example.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => Sut().HandleAsync(new LoginCommand("ghost@example.com", "x")));
    }

    [Fact]
    public async Task Throws_invalid_credentials_when_the_password_does_not_verify()
    {
        _users.GetByEmailAsync("jane@example.com", Arg.Any<CancellationToken>()).Returns(Existing());
        _hasher.Verify("wrong", "HASH", "SALT").Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => Sut().HandleAsync(new LoginCommand("jane@example.com", "wrong")));
    }

    [Fact]
    public async Task Returns_a_token_on_a_correct_password()
    {
        _users.GetByEmailAsync("jane@example.com", Arg.Any<CancellationToken>()).Returns(Existing());
        _hasher.Verify("right", "HASH", "SALT").Returns(true);
        _tokens.Create(Arg.Any<User>()).Returns(new AccessToken("jwt", DateTime.UtcNow.AddHours(1)));

        var result = await Sut().HandleAsync(new LoginCommand("Jane@Example.com", "right"));

        Assert.Equal("jwt", result.Token);
        Assert.Equal("jane@example.com", result.User.Email);
    }
}
