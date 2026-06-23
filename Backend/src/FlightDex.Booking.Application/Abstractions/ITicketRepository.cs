using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Persistence port for <see cref="Ticket"/>.</summary>
public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ticket>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int userId, int ticketId, CancellationToken cancellationToken = default);
}
