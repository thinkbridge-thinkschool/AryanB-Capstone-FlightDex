using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Flights.Application.Queries.GetFlightByCode;

internal sealed class GetFlightByCodeQueryHandler(IFlightRepository repository)
    : IQueryHandler<GetFlightByCodeQuery, IReadOnlyList<FlightDetail>>
{
    public async Task<IReadOnlyList<FlightDetail>> HandleAsync(
        GetFlightByCodeQuery query, CancellationToken cancellationToken = default)
    {
        var flights = await repository.GetByFlightCodeAsync(query.FlightCode, cancellationToken);
        return flights.Select(FlightDetail.FromDomain).ToList();
    }
}
