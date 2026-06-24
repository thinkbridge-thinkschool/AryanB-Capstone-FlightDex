using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Flights.Infrastructure.Persistence.Configurations;

/// <summary>
/// Maps <see cref="Location"/> to the Locations table — the search-suggestion lookup,
/// rebuilt from the timetable at startup. Per the project rule, the value is VARCHAR.
/// A unique index keeps the set deduplicated and serves the sorted read.
/// </summary>
internal sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.Value).HasColumnType("varchar(128)").IsRequired();

        builder.HasIndex(l => l.Value).IsUnique().HasDatabaseName("IX_Locations_Value");
    }
}
