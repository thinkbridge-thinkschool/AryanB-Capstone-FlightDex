using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlightDex.Flights.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef migrations` can build the context without running
/// the API host. Reads the same "FlightDex" connection string from the API's appsettings.
/// </summary>
public sealed class FlightsDbContextFactory : IDesignTimeDbContextFactory<FlightsDbContext>
{
    public FlightsDbContext CreateDbContext(string[] args)
    {
        // EF runs this from the Infrastructure project dir; hop to the API for config.
        var apiPath = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "FlightDex.Api"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString(DependencyInjection.ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{DependencyInjection.ConnectionStringName}' is not configured.");

        var options = new DbContextOptionsBuilder<FlightsDbContext>()
            .UseSqlite(connectionString)
            .Options;

        return new FlightsDbContext(options);
    }
}
