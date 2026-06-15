using FlightDex.Locations.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Locations.Infrastructure.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(ac => ac.Value, s => AirportCode.Create(s))
            .HasColumnName("Acode")
            .HasMaxLength(10);

        builder.OwnsOne(l => l.Address, a =>
        {
            a.Property(x => x.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            a.Property(x => x.State).HasColumnName("State").HasMaxLength(100).IsRequired();
            a.Property(x => x.Country).HasColumnName("Country").HasMaxLength(100).IsRequired();
        });
    }
}
