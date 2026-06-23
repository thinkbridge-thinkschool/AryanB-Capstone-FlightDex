using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Flights.Infrastructure.Persistence.Configurations;

/// <summary>
/// Maps <see cref="Flight"/> to the Flights table. Per the project rule, every column is
/// VARCHAR except <see cref="Flight.ScheduledTime"/>, a time. On SQLite both map to TEXT
/// affinity; the EF SQLite provider stores <see cref="TimeOnly"/> as "HH:mm:ss" text.
/// </summary>
internal sealed class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedOnAdd();

        builder.Property(f => f.Airport).HasColumnType("varchar(3)").IsRequired();

        // Enum persisted as text to keep "everything VARCHAR except times".
        builder.Property(f => f.Direction)
            .HasConversion<string>()
            .HasColumnType("varchar(16)")
            .IsRequired();

        builder.Property(f => f.ScheduledTime).IsRequired();

        builder.Property(f => f.AirlineCode).HasColumnType("varchar(8)").IsRequired();
        builder.Property(f => f.Airline).HasColumnType("varchar(128)").IsRequired();
        builder.Property(f => f.CounterpartAirport).HasColumnType("varchar(128)").IsRequired();
        builder.Property(f => f.CounterpartCode).HasColumnType("varchar(8)").IsRequired();
        builder.Property(f => f.CounterpartCity).HasColumnType("varchar(128)").IsRequired();
        builder.Property(f => f.FlightCode).HasColumnType("varchar(16)").IsRequired();
        builder.Property(f => f.Duration).HasColumnType("varchar(16)").IsRequired();

        // Covers the primary list query: an airport's departures/arrivals sorted by time.
        builder.HasIndex(f => new { f.Airport, f.Direction, f.ScheduledTime })
            .HasDatabaseName("IX_Flights_Airport_Direction_Time");

        // Covers to=/from= counterpart filtering and detail lookup by code.
        builder.HasIndex(f => f.CounterpartCode).HasDatabaseName("IX_Flights_CounterpartCode");
        builder.HasIndex(f => f.FlightCode).HasDatabaseName("IX_Flights_FlightCode");
    }
}
