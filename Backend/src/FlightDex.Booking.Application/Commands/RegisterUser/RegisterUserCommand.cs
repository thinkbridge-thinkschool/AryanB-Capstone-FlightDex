using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.RegisterUser;

/// <summary>Creates a new account and returns a token so the user is signed in immediately.</summary>
public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    int Age,
    bool IsGovernmentOfficial,
    bool IsLawEnforcementOrMilitary,
    string Password) : ICommand<AuthResult>;
