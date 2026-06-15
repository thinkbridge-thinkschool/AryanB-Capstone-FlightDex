using FlightDex.Locations.Infrastructure;
using FlightDex.Routing.Infrastructure;
using FlightDex.Timetable.Infrastructure;

namespace FlightDex.Api.Modules;

public static class ModuleRegistration
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddLocationsModule(configuration)
            .AddRoutingModule(configuration)
            .AddTimetableModule(configuration);

        return services;
    }
}
