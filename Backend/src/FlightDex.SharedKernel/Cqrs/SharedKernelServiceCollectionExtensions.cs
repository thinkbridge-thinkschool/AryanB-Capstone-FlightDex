using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.SharedKernel.Cqrs;

public static class SharedKernelServiceCollectionExtensions
{
    /// <summary>
    /// Registers the CQRS query and command dispatchers. Individual handlers are
    /// registered by each module (see the Flights and Booking DI extensions).
    /// </summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        return services;
    }
}
