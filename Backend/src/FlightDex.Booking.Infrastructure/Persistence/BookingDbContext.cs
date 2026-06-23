using FlightDex.Booking.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Booking.Infrastructure.Persistence;

/// <summary>EF Core context for the Booking module: Users and Tickets.</summary>
public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: apply UserConfiguration and TicketConfiguration from this assembly.
        base.OnModelCreating(modelBuilder);
    }
}
