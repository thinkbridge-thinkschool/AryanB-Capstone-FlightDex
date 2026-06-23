using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Persistence port for <see cref="User"/>.</summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
