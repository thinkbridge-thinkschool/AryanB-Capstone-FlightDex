using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Application.Queries.GetFlights;

internal sealed class GetFlightsQueryHandler(IFlightRepository flights)
    : IQueryHandler<GetFlightsQuery, PagedResult<FlightListItem>>
{
    public Task<PagedResult<FlightListItem>> HandleAsync(GetFlightsQuery query, CancellationToken cancellationToken = default)
    {
        // TODO: return flights.QueryAsync(query.Spec, cancellationToken).
        throw new NotImplementedException();
    }
}
