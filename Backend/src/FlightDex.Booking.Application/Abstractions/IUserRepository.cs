using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Write/read access to user accounts. EF Core lives in Infrastructure.</summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Adds the user and persists. The generated Id is populated on the entity.</summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
