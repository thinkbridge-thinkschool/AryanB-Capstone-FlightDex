using FlightDex.Routing.Application.Contracts;
using FlightDex.Routing.Domain;

namespace FlightDex.Routing.Application.Queries.GetRouteById;

/// <summary>Returns a route's from/to airport codes for a given route id.</summary>
public sealed class GetRouteByIdHandler
{
    private readonly IRouteRepository _repository;

    public GetRouteByIdHandler(IRouteRepository repository) => _repository = repository;

    public Task<RouteDto?> HandleAsync(GetRouteByIdQuery query, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
