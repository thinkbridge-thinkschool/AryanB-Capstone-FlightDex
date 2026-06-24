using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Infrastructure.Caching;
using FlightDex.Flights.Infrastructure.Persistence;
using FlightDex.Flights.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Flights.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "FlightDex";

    /// <summary>
    /// Registers the Flights module's persistence: the DbContext (SQLite), the read
    /// repository, and the CSV timetable seeder.
    /// </summary>
    public static IServiceCollection AddFlightsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<FlightsDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddMemoryCache(); // backs the static page-count cache in FlightRepository
        services.AddScoped<IFlightRepository, FlightRepository>();
        services.AddScoped<FlightTimetableSeeder>();

        // Airport suggestion cache, backed by the Locations table. The reader and the
        // startup rebuild both ride on the scoped FlightsDbContext.
        services.AddScoped<IAirportSuggestionCache, SqliteAirportSuggestionCache>();
        services.AddScoped<AirportSuggestionCacheBuilder>();

        return services;
    }
}
