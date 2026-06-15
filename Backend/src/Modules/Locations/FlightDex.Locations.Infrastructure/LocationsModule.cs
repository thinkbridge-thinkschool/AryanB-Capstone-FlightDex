using FlightDex.Locations.Application.Queries.GetLocationByCode;
using FlightDex.Locations.Domain;
using FlightDex.Locations.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Locations.Infrastructure;

public static class LocationsModule
{
    public static IServiceCollection AddLocationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LocationsDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("FlightDex")));

        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<GetLocationByCodeHandler>();

        return services;
    }
}
