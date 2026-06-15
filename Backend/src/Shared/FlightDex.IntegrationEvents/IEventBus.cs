namespace FlightDex.IntegrationEvents;

/// <summary>In-process event bus abstraction for publishing integration events.</summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
