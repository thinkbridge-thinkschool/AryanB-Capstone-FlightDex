using Azure.Identity;
using Azure.Messaging.ServiceBus;
using FlightDex.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Messaging;

public static class MessagingExtensions
{
    /// <summary>
    /// Registers the Service Bus client (managed-identity auth, no SAS key) and the
    /// publishing <see cref="IEventBus"/>. When no namespace is configured a no-op bus
    /// is registered so the app still runs locally without Service Bus.
    /// </summary>
    public static IServiceCollection AddServiceBusMessaging(
        this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ServiceBusOptions();
        configuration.GetSection(ServiceBusOptions.SectionName).Bind(options);
        services.AddSingleton(options);

        if (!options.IsConfigured)
        {
            services.AddSingleton<IEventBus, NoOpEventBus>();
            return services;
        }

        // DefaultAzureCredential resolves the App Service managed identity at runtime.
        services.AddSingleton(_ => new ServiceBusClient(
            options.FullyQualifiedNamespace, new DefaultAzureCredential()));
        services.AddSingleton<IEventBus, ServiceBusEventBus>();

        return services;
    }
}
