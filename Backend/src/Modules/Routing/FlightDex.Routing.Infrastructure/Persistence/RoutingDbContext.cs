using FlightDex.Routing.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Routing.Infrastructure.Persistence;

/// <summary>EF Core context owning the Routing schema.</summary>
public sealed class RoutingDbContext : DbContext
{
    public DbSet<Route> Routes => Set<Route>();

    public RoutingDbContext(DbContextOptions<RoutingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => throw new NotImplementedException();
}
