using FlightDex.Flights.Domain;
using FlightDex.Flights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightDex.Flights.Infrastructure.Caching;

/// <summary>
/// The suggestion-extract "script": reads every distinct destination/origin airport
/// (code, name, city) out of the timetable and (re)writes one row per airport into the
/// Locations table. Run once at startup, after the timetable has been seeded. Idempotent —
/// it replaces the table contents each time.
/// </summary>
public sealed class AirportSuggestionCacheBuilder(
    FlightsDbContext dbContext,
    ILogger<AirportSuggestionCacheBuilder> logger)
{
    public async Task RebuildAsync(CancellationToken cancellationToken = default)
    {
        // Pull every distinct counterpart (destination/origin) airport triple in one round-trip.
        var triples = await dbContext.Flights
            .Select(f => new { f.CounterpartCode, f.CounterpartAirport, f.CounterpartCity })
            .Distinct()
            .ToListAsync(cancellationToken);

        // Collapse to one row per code (case-insensitive), dropping blanks.
        var byCode = new Dictionary<string, Location>(StringComparer.OrdinalIgnoreCase);
        foreach (var t in triples)
        {
            var code = t.CounterpartCode?.Trim();
            if (string.IsNullOrEmpty(code) || byCode.ContainsKey(code)) continue;
            byCode[code] = new Location(code, t.CounterpartAirport?.Trim() ?? string.Empty,
                t.CounterpartCity?.Trim() ?? string.Empty);
        }

        // Replace the table contents so a re-run always reflects the current timetable.
        await dbContext.Locations.ExecuteDeleteAsync(cancellationToken);
        dbContext.Locations.AddRange(byCode.Values);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Cached {Count} airport suggestions in the Locations table.", byCode.Count);
    }
}
