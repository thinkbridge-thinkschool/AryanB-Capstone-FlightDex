namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// One airport search suggestion: its code, full name and city. The client renders these
/// as "Name [Code], City" and matches a typed code, name or city against any of the fields.
/// </summary>
public sealed record AirportSuggestion(string Code, string Name, string City);
