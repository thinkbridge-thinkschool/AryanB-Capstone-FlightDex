using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Routing.Infrastructure;

/// <summary>DI registration for the Routing context (db context, repository, handlers).</summary>
public static class RoutingModule
{
    public static IServiceCollection AddRoutingModule(
        this IServiceCollection services,
        IConfiguration configuration) => throw new NotImplementedException();
}
