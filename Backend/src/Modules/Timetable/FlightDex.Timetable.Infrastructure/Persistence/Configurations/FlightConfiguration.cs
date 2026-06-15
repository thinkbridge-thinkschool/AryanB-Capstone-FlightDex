using FlightDex.Timetable.Domain.Flight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for the Flight table (id, airline, route ref, schedule VO).</summary>
public sealed class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder) => throw new NotImplementedException();
}
