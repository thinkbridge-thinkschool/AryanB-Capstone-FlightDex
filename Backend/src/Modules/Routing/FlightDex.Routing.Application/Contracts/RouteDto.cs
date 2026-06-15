namespace FlightDex.Routing.Application.Contracts;

/// <summary>Route endpoint shape exposed to other contexts.</summary>
public sealed record RouteDto(
    Guid RouteId,
    string FromAirportCode,
    string ToAirportCode);
