using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Write/read access to booked tickets. EF Core lives in Infrastructure.</summary>
public interface ITicketRepository
{
    /// <summary>Adds the ticket and persists. The generated Id is populated on the entity.</summary>
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>A user's tickets, most recent first.</summary>
    Task<IReadOnlyList<Ticket>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>One ticket by id, or null. Used to check ownership before cancelling.</summary>
    Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task RemoveAsync(Ticket ticket, CancellationToken cancellationToken = default);
}
