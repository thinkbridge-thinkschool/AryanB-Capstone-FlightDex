using FlightDex.Routing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Routing.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for the Routes table (route id, from, to).</summary>
public sealed class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder) => throw new NotImplementedException();
}
