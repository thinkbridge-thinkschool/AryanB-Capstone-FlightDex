using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Flights.Infrastructure.Persistence.Configurations;

/// <summary>
/// Maps <see cref="Location"/> to the Locations table — the airport search-suggestion
/// lookup, rebuilt from the timetable at startup. One row per destination/origin airport
/// (code, name, city). Per the project rule the columns are VARCHAR; a unique index on the
/// code keeps the set deduplicated and serves the sorted read.
/// </summary>
internal sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.Code).HasColumnType("varchar(8)").IsRequired();
        builder.Property(l => l.Name).HasColumnType("varchar(128)").IsRequired();
        builder.Property(l => l.City).HasColumnType("varchar(128)").IsRequired();

        builder.HasIndex(l => l.Code).IsUnique().HasDatabaseName("IX_Locations_Code");
    }
}
