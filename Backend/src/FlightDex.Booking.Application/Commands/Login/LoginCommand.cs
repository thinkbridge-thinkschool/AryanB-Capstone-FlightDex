using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.Login;

/// <summary>Authenticates an existing account and returns a fresh token.</summary>
public sealed record LoginCommand(string Email, string Password) : ICommand<AuthResult>;
