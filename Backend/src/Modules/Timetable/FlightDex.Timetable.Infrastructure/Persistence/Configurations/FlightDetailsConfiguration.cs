using FlightDex.Timetable.Domain.Flight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

// FlightDetails is not persisted in the current schema; this class is retained as a placeholder.
public sealed class FlightDetailsConfiguration : IEntityTypeConfiguration<FlightDetails>
{
    public void Configure(EntityTypeBuilder<FlightDetails> builder) { }
}
