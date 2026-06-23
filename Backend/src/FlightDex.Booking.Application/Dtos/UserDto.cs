using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Dtos;

/// <summary>Public view of an account (never exposes the password hash/salt).</summary>
public sealed record UserDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    int Age)
{
    public static UserDto FromDomain(User u) =>
        throw new NotImplementedException(); // TODO: map from the User entity.
}
