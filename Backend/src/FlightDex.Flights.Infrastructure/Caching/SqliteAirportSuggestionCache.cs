using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Flights.Infrastructure.Caching;

/// <summary>
/// Reads airport suggestions from the Locations table. The table is populated by
/// <see cref="AirportSuggestionCacheBuilder"/> at startup; this type only reads.
/// </summary>
internal sealed class SqliteAirportSuggestionCache(FlightsDbContext dbContext) : IAirportSuggestionCache
{
    public async Task<IReadOnlyList<AirportSuggestion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Locations.AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new AirportSuggestion(l.Code, l.Name, l.City))
            .ToListAsync(cancellationToken);
    }
}
