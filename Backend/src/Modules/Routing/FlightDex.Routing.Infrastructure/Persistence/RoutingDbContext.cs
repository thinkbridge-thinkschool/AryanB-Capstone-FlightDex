using FlightDex.Routing.Domain;
using FlightDex.Routing.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Routing.Infrastructure.Persistence;

public sealed class RoutingDbContext : DbContext
{
    public DbSet<Route> Routes => Set<Route>();

    public RoutingDbContext(DbContextOptions<RoutingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RouteConfiguration());
    }
}
