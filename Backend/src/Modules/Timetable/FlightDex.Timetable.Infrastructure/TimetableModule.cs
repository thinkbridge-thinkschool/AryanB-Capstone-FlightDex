using FlightDex.Timetable.Application.Enrichment;
using FlightDex.Timetable.Application.EventHandlers;
using FlightDex.Timetable.Application.Queries.GetFlightDetails;
using FlightDex.Timetable.Application.Queries.GetFlightTimetable;
using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Infrastructure.Lookups;
using FlightDex.Timetable.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Timetable.Infrastructure;

public static class TimetableModule
{
    public static IServiceCollection AddTimetableModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TimetableDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("FlightDex")));

        services.AddScoped<IFlightRepository, FlightRepository>();
        services.AddScoped<IFlightViewReadStore, FlightViewReadStore>();

        services.AddScoped<ILocationLookup, LocationLookupAdapter>();
        services.AddScoped<IRouteLookup, RouteLookupAdapter>();
        services.AddScoped<FlightViewProjector>();

        services.AddScoped<GetFlightTimetableHandler>();
        services.AddScoped<GetFlightDetailsHandler>();
        services.AddScoped<LocationChangedHandler>();
        services.AddScoped<RouteChangedHandler>();

        return services;
    }
}
