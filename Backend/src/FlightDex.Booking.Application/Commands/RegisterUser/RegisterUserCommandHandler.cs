using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokens) : ICommandHandler<RegisterUserCommand, AuthResult>
{
    public Task<AuthResult> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: guard duplicate email, hash password, persist user, issue token.
        throw new NotImplementedException();
    }
}
