namespace FlightDex.Messaging;

/// <summary>Binds the <c>ServiceBus</c> configuration section. No connection string or SAS
/// key is ever present — the namespace hostname plus the app's managed identity is enough.</summary>
public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    /// <summary>e.g. <c>flightdex-bus-dev-xxxxxx.servicebus.windows.net</c>.</summary>
    public string? FullyQualifiedNamespace { get; set; }

    /// <summary>Integration-events queue name (matches the queue provisioned in Bicep).</summary>
    public string QueueName { get; set; } = "integration-events";

    /// <summary>False locally (no Service Bus); true once a namespace is configured.</summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(FullyQualifiedNamespace);
}
