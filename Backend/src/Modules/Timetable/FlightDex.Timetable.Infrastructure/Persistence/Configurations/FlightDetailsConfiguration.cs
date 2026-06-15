using FlightDex.Timetable.Domain.Flight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for the FlightDetails table (airplane, seating VO, avg duration).</summary>
public sealed class FlightDetailsConfiguration : IEntityTypeConfiguration<FlightDetails>
{
    public void Configure(EntityTypeBuilder<FlightDetails> builder) => throw new NotImplementedException();
}
