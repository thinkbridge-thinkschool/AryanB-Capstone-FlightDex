using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Flights.Infrastructure.Persistence;

/// <summary>
/// EF Core context owning the Flights module's tables. Each module owns its own
/// DbContext in this modular monolith; they share one physical database.
/// </summary>
public sealed class FlightsDbContext(DbContextOptions<FlightsDbContext> options) : DbContext(options)
{
    public DbSet<Flight> Flights => Set<Flight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlightsDbContext).Assembly);
    }
}
