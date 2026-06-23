using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Domain;
using FlightDex.SharedKernel.Paging;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Flights.Infrastructure.Persistence;

internal sealed class FlightRepository(FlightsDbContext dbContext) : IFlightRepository
{
    public async Task<PagedResult<Flight>> GetPagedAsync(
        FlightQuerySpec spec, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Flights.AsNoTracking()
            .Where(f => f.Direction == spec.Direction);

        if (!string.IsNullOrWhiteSpace(spec.Airport))
            query = query.Where(f => f.Airport == spec.Airport);

        if (!string.IsNullOrWhiteSpace(spec.CounterpartTerm))
        {
            // Match by destination/origin code OR city. The varchar columns use the
            // database's case-insensitive collation, so "DEL", "del" and "Delhi" all work.
            var term = spec.CounterpartTerm.Trim();
            query = query.Where(f => f.CounterpartCode == term || f.CounterpartCity == term);
        }

        if (spec.TimeAfter is { } after)
            query = query.Where(f => f.ScheduledTime >= after);

        if (spec.TimeBefore is { } before)
            query = query.Where(f => f.ScheduledTime <= before);

        // Count then fetch only the requested page — never the whole set.
        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(f => f.ScheduledTime)
            .ThenBy(f => f.FlightCode)
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Flight>(items, spec.Page, spec.PageSize, total);
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
