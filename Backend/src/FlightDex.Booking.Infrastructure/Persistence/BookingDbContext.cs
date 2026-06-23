using FlightDex.Booking.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Booking.Infrastructure.Persistence;

/// <summary>
/// EF Core context owning the Booking module's tables (Users, Tickets). Per the modular
/// monolith, this module owns its own context and database file, separate from Flights.
/// </summary>
public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
    }
}
