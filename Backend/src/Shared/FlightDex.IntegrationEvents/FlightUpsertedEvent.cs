namespace FlightDex.IntegrationEvents;

/// <summary>Raised when a flight is created or updated; drives FlightView projection.</summary>
public sealed record FlightUpsertedEvent(
    Guid FlightId,
    Guid EventId,
    DateTime OccurredOnUtc) : IIntegrationEvent;
