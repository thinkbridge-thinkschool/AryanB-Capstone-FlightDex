namespace FlightDex.Timetable.Application.Enrichment;

/// <summary>Port to resolve a RouteId into its from/to airport codes (Routing context).</summary>
public interface IRouteLookup
{
    Task<RouteLookupResult?> GetRouteAsync(Guid routeId, CancellationToken cancellationToken = default);
}

/// <summary>Route endpoints as seen by the Timetable enrichment pipeline.</summary>
public sealed record RouteLookupResult(
    Guid RouteId,
    string FromAirportCode,
    string ToAirportCode);
