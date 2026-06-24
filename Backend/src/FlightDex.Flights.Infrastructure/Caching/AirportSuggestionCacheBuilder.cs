using FlightDex.Flights.Domain;
using FlightDex.Flights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightDex.Flights.Infrastructure.Caching;

/// <summary>
/// The suggestion-extract "script": reads every unique airport code, airport name and city
/// out of the timetable and (re)writes them into the Locations table. Run once at startup,
/// after the timetable has been seeded. Idempotent — it replaces the table contents each time.
/// </summary>
public sealed class AirportSuggestionCacheBuilder(
    FlightsDbContext dbContext,
    ILogger<AirportSuggestionCacheBuilder> logger)
{
    public async Task RebuildAsync(CancellationToken cancellationToken = default)
    {
        // One UNION query pulls every distinct counterpart code, name and city plus the
        // served-airport codes. EF translates the chained Union into a single round-trip.
        var values = await dbContext.Flights.Select(f => f.CounterpartCode)
            .Union(dbContext.Flights.Select(f => f.CounterpartAirport))
            .Union(dbContext.Flights.Select(f => f.CounterpartCity))
            .Union(dbContext.Flights.Select(f => f.Airport))
            .ToListAsync(cancellationToken);

        // Keep unique appearances (case-insensitive), dropping blanks.
        var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var value in values)
        {
            var trimmed = value?.Trim();
            if (!string.IsNullOrEmpty(trimmed)) unique.Add(trimmed);
        }

        // Replace the table contents so a re-run always reflects the current timetable.
        await dbContext.Locations.ExecuteDeleteAsync(cancellationToken);
        dbContext.Locations.AddRange(unique.Select(v => new Location(v)));
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Cached {Count} airport suggestions in the Locations table.", unique.Count);
    }
}
