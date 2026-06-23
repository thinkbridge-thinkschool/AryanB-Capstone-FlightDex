using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Application.Abstractions;

/// <summary>Read port for the timetable.</summary>
public interface IFlightRepository
{
    Task<PagedResult<FlightListItem>> QueryAsync(FlightQuerySpec spec, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FlightDetail>> GetByCodeAsync(string flightCode, CancellationToken cancellationToken = default);
}
