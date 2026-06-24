namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// Read access to the cached set of airport search suggestions (codes, airport
/// names and cities). Backed by Redis, populated from the timetable database by a
/// one-off extract at startup. Suggestions never touch the main database; only the
/// actual flight search query does.
/// </summary>
public interface IAirportSuggestionCache
{
    /// <summary>All unique suggestion strings, sorted. Empty if the cache is unavailable.</summary>
    Task<IReadOnlyList<string>> GetAllAsync(CancellationToken cancellationToken = default);
}
