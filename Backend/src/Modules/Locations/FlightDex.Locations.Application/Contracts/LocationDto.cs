namespace FlightDex.Locations.Application.Contracts;

public sealed record LocationDto(
    string AirportCode,
    string City,
    string State,
    string Country);
