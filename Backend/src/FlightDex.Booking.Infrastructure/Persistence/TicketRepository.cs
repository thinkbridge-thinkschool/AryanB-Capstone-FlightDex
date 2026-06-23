using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Infrastructure.Persistence;

internal sealed class TicketRepository(BookingDbContext db) : ITicketRepository
{
    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<Ticket>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<bool> DeleteAsync(int userId, int ticketId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
