using FlightDex.Timetable.Domain.Airline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

public sealed class AirlineConfiguration : IEntityTypeConfiguration<Airline>
{
    public void Configure(EntityTypeBuilder<Airline> builder)
    {
        builder.ToTable("Airlines");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(aid => aid.Value, v => AirlineId.From(v))
            .HasColumnName("Aid");

        builder.Property(a => a.Name)
            .HasColumnName("Airline")
            .HasMaxLength(200)
            .IsRequired();
    }
}
