using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Flights.Application.Queries.GetFlightByCode;

internal sealed class GetFlightByCodeQueryHandler(IFlightRepository flights)
    : IQueryHandler<GetFlightByCodeQuery, IReadOnlyList<FlightDetail>>
{
    public Task<IReadOnlyList<FlightDetail>> HandleAsync(GetFlightByCodeQuery query, CancellationToken cancellationToken = default)
    {
        // TODO: return flights.GetByCodeAsync(query.FlightCode, cancellationToken).
        throw new NotImplementedException();
    }
}
