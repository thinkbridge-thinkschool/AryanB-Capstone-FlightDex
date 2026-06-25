namespace FlightDex.Flights.Domain;

/// <summary>
/// A single airport search suggestion derived from the timetable: a destination/origin
/// airport with its code, airport name and city. The set of locations is rebuilt from
/// <see cref="Flight"/> rows at startup and persisted in its own table, so type-ahead
/// lookups never have to scan the much larger Flights table.
/// </summary>
public sealed class Location
{
    public int Id { get; private set; }

    /// <summary>The airport IATA code, e.g. "DEL".</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>The full airport name, e.g. "Indira Gandhi International Airport".</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>The city the airport serves, e.g. "Delhi".</summary>
    public string City { get; private set; } = string.Empty;

    private Location() { } // EF Core

    public Location(string code, string name, string city)
    {
        Code = code;
        Name = name;
        City = city;
    }
}
