namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// Read access to the cached set of airport search suggestions (code, name and city per
/// airport). Backed by the Locations table, populated from the timetable by a one-off
/// extract at startup, so type-aheads never scan the larger Flights table.
/// </summary>
public interface IAirportSuggestionCache
{
    /// <summary>All airport suggestions, sorted by name. Empty if the cache is unavailable.</summary>
    Task<IReadOnlyList<AirportSuggestion>> GetAllAsync(CancellationToken cancellationToken = default);
}
