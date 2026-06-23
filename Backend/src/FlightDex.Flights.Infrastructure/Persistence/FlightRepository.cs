using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Infrastructure.Persistence;

/// <summary><see cref="IFlightRepository"/> over <see cref="FlightsDbContext"/>.</summary>
internal sealed class FlightRepository(FlightsDbContext db) : IFlightRepository
{
    public Task<PagedResult<FlightListItem>> QueryAsync(FlightQuerySpec spec, CancellationToken cancellationToken = default)
    {
        // TODO: build the filtered/sorted/paged query and project to FlightListItem.
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<FlightDetail>> GetByCodeAsync(string flightCode, CancellationToken cancellationToken = default)
    {
        // TODO: load matching legs by flight code and project to FlightDetail.
        throw new NotImplementedException();
    }
}
