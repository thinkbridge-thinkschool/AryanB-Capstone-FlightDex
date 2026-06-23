using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokens) : ICommandHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: load by email, verify password, throw InvalidCredentialsException on mismatch, issue token.
        throw new NotImplementedException();
    }
}
