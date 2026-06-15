using FlightDex.Routing.Application.Queries.GetRouteById;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Infrastructure.Lookups;

/// <summary><see cref="IRouteLookup"/> implementation calling the Routing context's query API.</summary>
public sealed class RouteLookupAdapter : IRouteLookup
{
    private readonly GetRouteByIdHandler _getRouteById;

    public RouteLookupAdapter(GetRouteByIdHandler getRouteById) => _getRouteById = getRouteById;

    public Task<RouteLookupResult?> GetRouteAsync(Guid routeId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
