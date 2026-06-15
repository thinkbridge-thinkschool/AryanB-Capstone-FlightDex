using FlightDex.Timetable.Domain.Flight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

public sealed class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasConversion(fid => fid.Value, v => FlightId.From(v))
            .HasColumnName("Fid");

        builder.Property(f => f.AirlineId).HasColumnName("Aid");
        builder.Property(f => f.RouteId).HasColumnName("Rid");

        builder.OwnsOne(f => f.Schedule, s =>
        {
            s.Property(x => x.DepartureUtc).HasColumnName("Departure").IsRequired();
            s.Property(x => x.ArrivalUtc).HasColumnName("Arrival").IsRequired();
        });

        builder.Property(f => f.Status)
            .HasColumnName("Status")
            .HasMaxLength(20)
            .IsRequired();
    }
}
