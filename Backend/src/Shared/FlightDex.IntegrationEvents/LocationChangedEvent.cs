namespace FlightDex.IntegrationEvents;

/// <summary>Raised when a location's details change; refreshes affected FlightViews.</summary>
public sealed record LocationChangedEvent(
    string AirportCode,
    Guid EventId,
    DateTime OccurredOnUtc) : IIntegrationEvent;
