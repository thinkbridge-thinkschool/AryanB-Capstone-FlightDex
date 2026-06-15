using FlightDex.Timetable.Domain.Flight;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Timetable.Infrastructure.Persistence;

public sealed class FlightRepository : IFlightRepository
{
    private readonly TimetableDbContext _dbContext;

    public FlightRepository(TimetableDbContext dbContext) => _dbContext = dbContext;

    public Task<Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken = default)
    {
        var val = id.Value;
        return _dbContext.Flights
            .FirstOrDefaultAsync(f => f.Id == FlightId.From(val), cancellationToken);
    }

    public async Task AddAsync(Flight flight, CancellationToken cancellationToken = default)
        => await _dbContext.Flights.AddAsync(flight, cancellationToken);

    public void Update(Flight flight)
        => _dbContext.Flights.Update(flight);
}
