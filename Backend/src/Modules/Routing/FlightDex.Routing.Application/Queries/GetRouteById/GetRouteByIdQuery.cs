namespace FlightDex.Routing.Application.Queries.GetRouteById;

/// <summary>Route lookup input (route id).</summary>
public sealed record GetRouteByIdQuery(Guid RouteId);
