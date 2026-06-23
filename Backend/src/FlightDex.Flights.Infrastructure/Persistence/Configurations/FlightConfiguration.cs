using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Flights.Infrastructure.Persistence.Configurations;

/// <summary>EF mapping for the <see cref="Flight"/> timetable row.</summary>
internal sealed class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        // TODO: map table name, key, columns, and the Direction enum conversion.
        throw new NotImplementedException();
    }
}
