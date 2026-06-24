using FlightDex.Flights.Application.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FlightDex.Flights.Infrastructure.Caching;

/// <summary>
/// Reads airport suggestions from a Redis set. The set is populated by
/// <see cref="AirportSuggestionCacheBuilder"/> at startup; this type only reads.
/// </summary>
internal sealed class RedisAirportSuggestionCache(
    IConnectionMultiplexer redis,
    ILogger<RedisAirportSuggestionCache> logger) : IAirportSuggestionCache
{
    internal const string SetKey = "flightdex:airport-suggestions";

    public async Task<IReadOnlyList<string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            var members = await db.SetMembersAsync(SetKey);
            return members
                .Select(m => m.ToString())
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch (RedisConnectionException ex)
        {
            logger.LogWarning(ex, "Redis unavailable; returning no airport suggestions.");
            return [];
        }
    }
}
