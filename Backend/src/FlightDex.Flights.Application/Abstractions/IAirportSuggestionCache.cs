namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// Read access to the cached set of airport search suggestions (codes, airport
/// names and cities). Backed by the Locations table, populated from the timetable by
/// a one-off extract at startup, so type-aheads never scan the larger Flights table.
/// </summary>
public interface IAirportSuggestionCache
{
    /// <summary>All unique suggestion strings, sorted. Empty if the cache is unavailable.</summary>
    Task<IReadOnlyList<string>> GetAllAsync(CancellationToken cancellationToken = default);
}
