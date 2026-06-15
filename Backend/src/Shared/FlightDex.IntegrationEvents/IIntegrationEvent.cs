namespace FlightDex.IntegrationEvents;

/// <summary>Marker for integration events crossing context boundaries.</summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTime OccurredOnUtc { get; }
}
