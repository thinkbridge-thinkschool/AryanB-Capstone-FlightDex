using FlightDex.Locations.Domain;
using FlightDex.Locations.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Locations.Infrastructure.Persistence;

public sealed class LocationsDbContext : DbContext
{
    public DbSet<Location> Locations => Set<Location>();

    public LocationsDbContext(DbContextOptions<LocationsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
    }
}
