using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Infrastructure.Caching;
using FlightDex.Flights.Infrastructure.Persistence;
using FlightDex.Flights.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

        // Redis-backed airport suggestion cache. AbortOnConnectFail=false lets the API
        // start (and keep serving searches) even when Redis is unreachable.
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        var redisOptions = ConfigurationOptions.Parse(redisConnection);
        redisOptions.AbortOnConnectFail = false;
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisOptions));
        services.AddSingleton<IAirportSuggestionCache, RedisAirportSuggestionCache>();
        services.AddScoped<AirportSuggestionCacheBuilder>();

        return services;
    }
}
