using FlightDex.Locations.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Locations.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for the Locations table (code, airport name, address).</summary>
public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder) => throw new NotImplementedException();
}
