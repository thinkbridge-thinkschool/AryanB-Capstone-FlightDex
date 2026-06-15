using FlightDex.Timetable.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

public sealed class FlightViewConfiguration : IEntityTypeConfiguration<FlightView>
{
    public void Configure(EntityTypeBuilder<FlightView> builder)
    {
        builder.ToTable("FlightViews");
        builder.HasKey(v => v.FlightId);

        builder.Property(v => v.Airline).HasMaxLength(200).IsRequired();
        builder.Property(v => v.FromAirportCode).HasMaxLength(10).IsRequired();
        builder.Property(v => v.FromCity).HasMaxLength(100).IsRequired();
        builder.Property(v => v.FromState).HasMaxLength(100).IsRequired();
        builder.Property(v => v.FromCountry).HasMaxLength(100).IsRequired();
        builder.Property(v => v.ToAirportCode).HasMaxLength(10).IsRequired();
        builder.Property(v => v.ToCity).HasMaxLength(100).IsRequired();
        builder.Property(v => v.ToState).HasMaxLength(100).IsRequired();
        builder.Property(v => v.ToCountry).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Status).HasMaxLength(20).IsRequired();
    }
}
