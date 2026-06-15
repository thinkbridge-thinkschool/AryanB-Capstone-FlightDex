namespace FlightDex.Timetable.Domain.Flight;

/// <summary>Persistence port for the Flight aggregate.</summary>
public interface IFlightRepository
{
    Task<Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken = default);

    Task AddAsync(Flight flight, CancellationToken cancellationToken = default);

    void Update(Flight flight);
}
