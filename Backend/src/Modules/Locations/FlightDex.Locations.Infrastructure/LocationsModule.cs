using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Locations.Infrastructure;

/// <summary>DI registration for the Locations context (db context, repository, handlers).</summary>
public static class LocationsModule
{
    public static IServiceCollection AddLocationsModule(
        this IServiceCollection services,
        IConfiguration configuration) => throw new NotImplementedException();
}
