using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Infrastructure.Persistence;

internal sealed class UserRepository(BookingDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
