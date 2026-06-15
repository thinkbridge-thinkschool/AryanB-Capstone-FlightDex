namespace FlightDex.Locations.Application.Contracts;

/// <summary>Location detail shape exposed to other contexts.</summary>
public sealed record LocationDto(
    string AirportCode,
    string AirportName,
    string City,
    string State,
    string Country);
