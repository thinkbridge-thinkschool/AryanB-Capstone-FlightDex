namespace FlightDex.Booking.Application.Dtos;

/// <summary>
/// What the client receives after a successful register or login: the bearer token to
/// send on subsequent requests, when it expires, and the signed-in user's details.
/// </summary>
public sealed record AuthResult(
    string Token,
    DateTime ExpiresAtUtc,
    UserDto User);
