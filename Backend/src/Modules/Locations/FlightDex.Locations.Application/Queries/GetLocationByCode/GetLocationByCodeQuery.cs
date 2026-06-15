namespace FlightDex.Locations.Application.Queries.GetLocationByCode;

/// <summary>Location lookup input (airport code).</summary>
public sealed record GetLocationByCodeQuery(string AirportCode);
