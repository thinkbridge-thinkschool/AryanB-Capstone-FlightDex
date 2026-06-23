using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.Booking.Domain;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokens) : ICommandHandler<RegisterUserCommand, AuthResult>
{
    public async Task<AuthResult> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        if (await users.EmailExistsAsync(email, cancellationToken))
            throw new EmailAlreadyInUseException(email);

        var (hash, salt) = passwordHasher.Hash(command.Password);

        var user = new User(
            email,
            command.FirstName.Trim(),
            command.LastName.Trim(),
            command.Age,
            command.IsGovernmentOfficial,
            command.IsLawEnforcementOrMilitary,
            hash,
            salt);

        await users.AddAsync(user, cancellationToken);

        var token = tokens.Create(user);
        return new AuthResult(token.Token, token.ExpiresAtUtc, UserDto.FromDomain(user));
    }
}
