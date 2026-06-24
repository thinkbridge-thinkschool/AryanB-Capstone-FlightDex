namespace FlightDex.Flights.Domain;

/// <summary>
/// A single search suggestion derived from the timetable: a unique airport code,
/// airport name or city. The set of locations is rebuilt from <see cref="Flight"/>
/// rows at startup and persisted in its own table, so type-ahead lookups never have
/// to scan the much larger Flights table.
/// </summary>
public sealed class Location
{
    public int Id { get; private set; }

    /// <summary>The suggestion text — an airport code, airport name or city.</summary>
    public string Value { get; private set; } = string.Empty;

    private Location() { } // EF Core

    public Location(string value)
    {
        Value = value;
    }
}
