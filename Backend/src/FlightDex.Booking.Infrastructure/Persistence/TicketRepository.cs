using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Booking.Infrastructure.Persistence;

internal sealed class TicketRepository(BookingDbContext dbContext) : ITicketRepository
{
    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetByUserAsync(int userId, CancellationToken cancellationToken = default) =>
        await dbContext.Tickets.AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Id) // most recently booked first
            .ToListAsync(cancellationToken);

    // Tracked (no AsNoTracking) so the entity can be removed directly.
    public Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        // The ticket comes from GetByIdAsync (tracked), so saving flushes its changes.
        dbContext.Tickets.Update(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        dbContext.Tickets.Remove(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
