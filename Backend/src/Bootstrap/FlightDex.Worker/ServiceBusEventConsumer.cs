using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FlightDex.IntegrationEvents;
using FlightDex.Messaging;

namespace FlightDex.Worker;

/// <summary>
/// Background consumer for the integration-events queue. The Azure SDK starts a "process"
/// span for each message and continues the trace carried in the message, so this work shows
/// up in App Insights as a child of the API request that published the event.
/// </summary>
public sealed class ServiceBusEventConsumer : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ServiceBusEventConsumer> _logger;
    private ServiceBusProcessor? _processor;

    public ServiceBusEventConsumer(
        ServiceBusClient client,
        ServiceBusOptions options,
        IServiceScopeFactory scopeFactory,
        ILogger<ServiceBusEventConsumer> logger)
    {
        _client = client;
        _options = options;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor(_options.QueueName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls   = 1,
            AutoCompleteMessages = false
        });

        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync   += OnErrorAsync;

        _logger.LogInformation("Starting Service Bus consumer on queue {Queue}", _options.QueueName);
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var eventType = args.Message.Subject;
        var body = args.Message.Body.ToString();

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            switch (eventType)
            {
                case nameof(FlightUpsertedEvent):
                    var evt = JsonSerializer.Deserialize<FlightUpsertedEvent>(body)
                        ?? throw new InvalidOperationException("Empty FlightUpsertedEvent payload");
                    var projection = scope.ServiceProvider.GetRequiredService<FlightViewProjectionService>();
                    await projection.RebuildAsync(evt.FlightId, args.CancellationToken);
                    break;

                default:
                    _logger.LogWarning("Unhandled event type {EventType}; completing message", eventType);
                    break;
            }

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            // Abandon so the message is retried (and dead-lettered after maxDeliveryCount).
            _logger.LogError(ex, "Failed to process {EventType} message {MessageId}", eventType, args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Service Bus error in {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
