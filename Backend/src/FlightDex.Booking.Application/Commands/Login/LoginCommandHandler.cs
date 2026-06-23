using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokens) : ICommandHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(email, cancellationToken);

        // Same error whether the email is unknown or the password is wrong — don't leak
        // which accounts exist.
        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidCredentialsException();

        var token = tokens.Create(user);
        return new AuthResult(token.Token, token.ExpiresAtUtc, UserDto.FromDomain(user));
    }
}
