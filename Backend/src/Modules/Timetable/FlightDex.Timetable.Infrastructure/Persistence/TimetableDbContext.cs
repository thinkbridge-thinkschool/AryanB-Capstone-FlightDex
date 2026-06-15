using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Timetable.Infrastructure.Persistence;

/// <summary>EF Core context for Flight, FlightDetails, and the FlightView read model.</summary>
public sealed class TimetableDbContext : DbContext
{
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<FlightView> FlightViews => Set<FlightView>();

    public TimetableDbContext(DbContextOptions<TimetableDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => throw new NotImplementedException();
}
