using FlightDex.Routing.Application.Contracts;
using FlightDex.Routing.Domain;

namespace FlightDex.Routing.Application.Queries.GetRouteById;

public sealed class GetRouteByIdHandler
{
    private readonly IRouteRepository _repository;

    public GetRouteByIdHandler(IRouteRepository repository) => _repository = repository;

    public async Task<RouteDto?> HandleAsync(GetRouteByIdQuery query, CancellationToken cancellationToken = default)
    {
        var id = RouteId.From(query.RouteId);
        var route = await _repository.GetByIdAsync(id, cancellationToken);
        if (route is null) return null;
        return new RouteDto(route.Id.Value, route.From.Value, route.To.Value);
    }
}
