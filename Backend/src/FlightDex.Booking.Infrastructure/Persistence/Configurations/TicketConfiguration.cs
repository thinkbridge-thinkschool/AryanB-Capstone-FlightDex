using FlightDex.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightDex.Booking.Infrastructure.Persistence.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.UserId).IsRequired();

        // DateOnly / TimeOnly map to TEXT on SQLite ("yyyy-MM-dd" / "HH:mm:ss").
        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.Time).IsRequired();

        builder.Property(t => t.OriginCode).HasColumnType("varchar(8)").IsRequired();
        builder.Property(t => t.OriginAirport).HasColumnType("varchar(128)").IsRequired();
        builder.Property(t => t.OriginCity).HasColumnType("varchar(128)").IsRequired();
        builder.Property(t => t.DestinationCode).HasColumnType("varchar(8)").IsRequired();
        builder.Property(t => t.DestinationAirport).HasColumnType("varchar(128)").IsRequired();
        builder.Property(t => t.DestinationCity).HasColumnType("varchar(128)").IsRequired();

        builder.Property(t => t.FirstName).HasColumnType("varchar(128)").IsRequired();
        builder.Property(t => t.LastName).HasColumnType("varchar(128)").IsRequired();
        builder.Property(t => t.Age).IsRequired();

        // "My Tickets" lists a single user's tickets.
        builder.HasIndex(t => t.UserId).HasDatabaseName("IX_Tickets_UserId");
    }
}
