namespace FlightDex.IntegrationEvents;

/// <summary>Raised when a route's endpoints change; refreshes affected FlightViews.</summary>
public sealed record RouteChangedEvent(
    Guid RouteId,
    Guid EventId,
    DateTime OccurredOnUtc) : IIntegrationEvent;
