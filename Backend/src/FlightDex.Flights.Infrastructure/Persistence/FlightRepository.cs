using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Domain;
using FlightDex.SharedKernel.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FlightDex.Flights.Infrastructure.Persistence;

internal sealed class FlightRepository(FlightsDbContext dbContext, IMemoryCache cache) : IFlightRepository
{
    public async Task<PagedResult<Flight>> GetPagedAsync(
        FlightQuerySpec spec, CancellationToken cancellationToken = default)
    {
        // The timetable is static for the life of the process, so a given (filter + page)
        // always yields the same page. Cache the whole result — on a repeat request we skip
        // SQLite (both the COUNT and the items query) and the EF materialisation entirely.
        var cacheKey = $"flight-page:{spec.Direction}:{spec.Airport}:{spec.CounterpartTerm}:" +
                       $"{spec.TimeAfter}:{spec.TimeBefore}:{spec.Page}:{spec.PageSize}";
        if (cache.TryGetValue(cacheKey, out PagedResult<Flight>? cached) && cached is not null)
            return cached;

        var query = dbContext.Flights.AsNoTracking()
            .Where(f => f.Direction == spec.Direction);

        if (!string.IsNullOrWhiteSpace(spec.Airport))
            query = query.Where(f => f.Airport == spec.Airport);

        if (!string.IsNullOrWhiteSpace(spec.CounterpartTerm))
        {
            // Match by destination/origin code, city OR airport name — so any suggestion
            // (which may be a code, a city or a full airport name) works as a search term.
            // The varchar columns use a case-insensitive collation: "DEL", "del", "Delhi" all work.
            var term = spec.CounterpartTerm.Trim();
            query = query.Where(f =>
                f.CounterpartCode == term || f.CounterpartCity == term || f.CounterpartAirport == term);
        }

        if (spec.TimeAfter is { } after)
            query = query.Where(f => f.ScheduledTime >= after);

        if (spec.TimeBefore is { } before)
            query = query.Where(f => f.ScheduledTime <= before);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(f => f.ScheduledTime)
            .ThenBy(f => f.FlightCode)
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<Flight>(items, spec.Page, spec.PageSize, total);
        cache.Set(cacheKey, result);
        return result;
    }

    public async Task<IReadOnlyList<Flight>> GetByFlightCodeAsync(
        string flightCode, CancellationToken cancellationToken = default)
    {
        return await dbContext.Flights.AsNoTracking()
            .Where(f => f.FlightCode == flightCode)
            .OrderBy(f => f.Direction)
            .ThenBy(f => f.ScheduledTime)
            .ToListAsync(cancellationToken);
    }
}
