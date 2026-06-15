using FlightDex.SharedKernel.Pagination;
using FlightDex.Timetable.Application.Queries.GetFlightTimetable;
using FlightDex.Timetable.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Timetable.Infrastructure.Persistence;

public sealed class FlightViewReadStore : IFlightViewReadStore
{
    private readonly TimetableDbContext _dbContext;

    public FlightViewReadStore(TimetableDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<FlightView>> QueryAsync(
        GetFlightTimetableQuery query,
        CancellationToken cancellationToken = default)
    {
        var q = _dbContext.FlightViews.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.DepartureAirportCode))
            q = q.Where(v => v.FromAirportCode == query.DepartureAirportCode.ToUpperInvariant());

        if (!string.IsNullOrWhiteSpace(query.ArrivalAirportCode))
            q = q.Where(v => v.ToAirportCode == query.ArrivalAirportCode.ToUpperInvariant());

        if (!string.IsNullOrWhiteSpace(query.AirportCode))
        {
            var code = query.AirportCode.ToUpperInvariant();
            q = q.Where(v => v.FromAirportCode == code || v.ToAirportCode == code);
        }

        if (query.FromUtc.HasValue)
            q = q.Where(v => v.DepartureUtc >= query.FromUtc.Value);

        if (query.ToUtc.HasValue)
            q = q.Where(v => v.DepartureUtc <= query.ToUtc.Value);

        q = query.SortBy switch
        {
            FlightSortBy.Arrival  => q.OrderBy(v => v.ArrivalUtc),
            FlightSortBy.Airline  => q.OrderBy(v => v.Airline),
            FlightSortBy.Location => q.OrderBy(v => v.FromCity),
            _                     => q.OrderBy(v => v.DepartureUtc)
        };

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .Skip((query.Page.PageNumber - 1) * query.Page.PageSize)
            .Take(query.Page.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<FlightView>(items, query.Page.PageNumber, query.Page.PageSize, total);
    }

    public Task<FlightView?> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default)
        => _dbContext.FlightViews.AsNoTracking()
            .FirstOrDefaultAsync(v => v.FlightId == flightId, cancellationToken);
}
