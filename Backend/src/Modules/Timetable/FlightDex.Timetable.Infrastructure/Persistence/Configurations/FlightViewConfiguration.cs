using FlightDex.Timetable.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Timetable.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for the denormalized FlightView read-model table.</summary>
public sealed class FlightViewConfiguration : IEntityTypeConfiguration<FlightView>
{
    public void Configure(EntityTypeBuilder<FlightView> builder) => throw new NotImplementedException();
}
