using FlightDex.Booking.Application;
using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Commands.RegisterUser;
using FlightDex.Booking.Domain;
using NSubstitute;

namespace FlightDex.UnitTests.Booking;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _tokens = Substitute.For<IJwtTokenService>();

    private RegisterUserCommandHandler Sut() => new(_users, _hasher, _tokens);

    private static RegisterUserCommand Command(string email = "Jane@Example.com") =>
        new(email, "Jane", "Doe", 30, false, false, "p@ssw0rd");

    [Fact]
    public async Task Throws_when_the_email_already_has_an_account()
    {
        _users.EmailExistsAsync("jane@example.com", Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => Sut().HandleAsync(Command()));
        await _users.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Normalizes_email_hashes_password_and_persists_the_user()
    {
        _users.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _hasher.Hash("p@ssw0rd").Returns(("HASH", "SALT"));
        _tokens.Create(Arg.Any<User>()).Returns(new AccessToken("jwt", DateTime.UtcNow.AddHours(1)));

        User? persisted = null;
        await _users.AddAsync(Arg.Do<User>(u => persisted = u), Arg.Any<CancellationToken>());

        var result = await Sut().HandleAsync(Command("Jane@Example.com"));

        Assert.NotNull(persisted);
        Assert.Equal("jane@example.com", persisted!.Email);   // lower-cased + trimmed
        Assert.Equal("HASH", persisted.PasswordHash);
        Assert.Equal("SALT", persisted.PasswordSalt);
        Assert.Equal("jwt", result.Token);
        Assert.Equal("jane@example.com", result.User.Email);
    }
}
