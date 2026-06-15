using FlightDex.Locations.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Locations.Infrastructure.Persistence;

/// <summary>EF Core context owning the Locations schema.</summary>
public sealed class LocationsDbContext : DbContext
{
    public DbSet<Location> Locations => Set<Location>();

    public LocationsDbContext(DbContextOptions<LocationsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => throw new NotImplementedException();
}
