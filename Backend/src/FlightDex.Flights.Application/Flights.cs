namespace FlightDex.Flights.Application;

/// <summary>The airports FlightDex serves. "at" must be one of these.</summary>
public static class ServedAirports
{
    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "BLR", "BOM", "PNQ" };

    public static bool IsServed(string? code) => code is not null && All.Contains(code);
}
