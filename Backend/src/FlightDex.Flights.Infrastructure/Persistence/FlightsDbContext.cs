using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Flights.Infrastructure.Persistence;

/// <summary>EF Core context for the unified Flights timetable table.</summary>
public sealed class FlightsDbContext(DbContextOptions<FlightsDbContext> options) : DbContext(options)
{
    public DbSet<Flight> Flights => Set<Flight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: apply configurations from this assembly (FlightConfiguration).
        base.OnModelCreating(modelBuilder);
    }
}
