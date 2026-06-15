using FlightDex.Routing.Application.Queries.GetRouteById;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Infrastructure.Lookups;

public sealed class RouteLookupAdapter : IRouteLookup
{
    private readonly GetRouteByIdHandler _getRouteById;

    public RouteLookupAdapter(GetRouteByIdHandler getRouteById) => _getRouteById = getRouteById;

    public async Task<RouteLookupResult?> GetRouteAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        var dto = await _getRouteById.HandleAsync(new GetRouteByIdQuery(routeId), cancellationToken);
        if (dto is null) return null;
        return new RouteLookupResult(dto.RouteId, dto.FromAirportCode, dto.ToAirportCode);
    }
}
