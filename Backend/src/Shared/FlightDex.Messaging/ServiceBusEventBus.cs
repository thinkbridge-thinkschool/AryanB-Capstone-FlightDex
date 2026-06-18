using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FlightDex.IntegrationEvents;

namespace FlightDex.Messaging;

/// <summary>
/// Publishes integration events to the Service Bus queue. The Azure SDK injects the
/// current trace context (<c>traceparent</c>) into the message, so the worker's consume
/// span links back to the API request that produced it — the API → worker trace stitch.
/// </summary>
public sealed class ServiceBusEventBus : IEventBus, IAsyncDisposable
{
    private readonly ServiceBusSender _sender;

    public ServiceBusEventBus(ServiceBusClient client, ServiceBusOptions options)
        => _sender = client.CreateSender(options.QueueName);

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var message = new ServiceBusMessage(JsonSerializer.SerializeToUtf8Bytes(integrationEvent))
        {
            MessageId   = integrationEvent.EventId.ToString(),
            Subject     = typeof(TEvent).Name,   // consumer dispatches on this
            ContentType = "application/json"
        };

        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public ValueTask DisposeAsync() => _sender.DisposeAsync();
}
