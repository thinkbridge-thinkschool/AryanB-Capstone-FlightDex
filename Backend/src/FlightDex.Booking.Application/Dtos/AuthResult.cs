namespace FlightDex.Booking.Application.Dtos;

/// <summary>The token + user returned by register/login.</summary>
public sealed record AuthResult(string Token, UserDto User);
