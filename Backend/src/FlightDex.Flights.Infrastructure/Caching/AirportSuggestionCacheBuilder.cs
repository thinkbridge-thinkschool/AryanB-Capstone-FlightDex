using FlightDex.Flights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FlightDex.Flights.Infrastructure.Caching;

/// <summary>
/// The cache-extract "script": reads every unique airport code, airport name and city
/// out of the timetable database and (re)writes them into the Redis suggestion set.
/// Run once at startup, after the timetable has been seeded. Idempotent — it replaces
/// the set each time.
/// </summary>
public sealed class AirportSuggestionCacheBuilder(
    FlightsDbContext dbContext,
    IConnectionMultiplexer redis,
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

        try
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(RedisAirportSuggestionCache.SetKey);
            if (unique.Count > 0)
            {
                var members = unique.Select(s => (RedisValue)s).ToArray();
                await db.SetAddAsync(RedisAirportSuggestionCache.SetKey, members);
            }
            logger.LogInformation("Cached {Count} airport suggestions in Redis.", unique.Count);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable; airport suggestion cache not built.");
        }
    }
}
