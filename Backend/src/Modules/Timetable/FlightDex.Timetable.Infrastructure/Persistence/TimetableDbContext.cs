using FlightDex.Timetable.Domain.Airline;
using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Domain.ReadModels;
using FlightDex.Timetable.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Timetable.Infrastructure.Persistence;

public sealed class TimetableDbContext : DbContext
{
    public DbSet<Airline> Airlines => Set<Airline>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<FlightView> FlightViews => Set<FlightView>();

    public TimetableDbContext(DbContextOptions<TimetableDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AirlineConfiguration());
        modelBuilder.ApplyConfiguration(new FlightConfiguration());
        modelBuilder.ApplyConfiguration(new FlightViewConfiguration());
    }
}
