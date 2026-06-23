using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Dtos;

/// <summary>Public view of a user — never includes the password hash.</summary>
public sealed record UserDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    int Age,
    bool IsGovernmentOfficial,
    bool IsLawEnforcementOrMilitary)
{
    public static UserDto FromDomain(User u) => new(
        u.Id, u.Email, u.FirstName, u.LastName, u.Age,
        u.IsGovernmentOfficial, u.IsLawEnforcementOrMilitary);
}
