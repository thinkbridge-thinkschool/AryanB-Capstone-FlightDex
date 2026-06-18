using FlightDex.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace FlightDex.Messaging;

/// <summary>Registered when no Service Bus namespace is configured (local dev) so the
/// publish path still works — the event is logged and dropped instead of sent.</summary>
public sealed class NoOpEventBus : IEventBus
{
    private readonly ILogger<NoOpEventBus> _logger;

    public NoOpEventBus(ILogger<NoOpEventBus> logger) => _logger = logger;

    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        _logger.LogInformation(
            "Service Bus not configured; skipped publishing {EventType} {EventId}",
            typeof(TEvent).Name, integrationEvent.EventId);
        return Task.CompletedTask;
    }
}
