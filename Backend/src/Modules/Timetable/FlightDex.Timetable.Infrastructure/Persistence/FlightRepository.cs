using FlightDex.Timetable.Domain.Flight;

namespace FlightDex.Timetable.Infrastructure.Persistence;

/// <summary><see cref="IFlightRepository"/> implementation backed by EF Core.</summary>
public sealed class FlightRepository : IFlightRepository
{
    private readonly TimetableDbContext _dbContext;

    public FlightRepository(TimetableDbContext dbContext) => _dbContext = dbContext;

    public Task<Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddAsync(Flight flight, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public void Update(Flight flight) => throw new NotImplementedException();
}
