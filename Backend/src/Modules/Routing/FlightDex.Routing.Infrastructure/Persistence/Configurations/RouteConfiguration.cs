using FlightDex.Routing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Routing.Infrastructure.Persistence.Configurations;

public sealed class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(rid => rid.Value, v => RouteId.From(v))
            .HasColumnName("Rid");

        builder.OwnsOne(r => r.From, f =>
        {
            f.Property(x => x.Value).HasColumnName("From").HasMaxLength(10).IsRequired();
        });

        builder.OwnsOne(r => r.To, t =>
        {
            t.Property(x => x.Value).HasColumnName("To").HasMaxLength(10).IsRequired();
        });
    }
}
