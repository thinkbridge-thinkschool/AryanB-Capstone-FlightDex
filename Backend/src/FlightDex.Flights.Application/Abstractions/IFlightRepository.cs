using FlightDex.Flights.Domain;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// Read-side access to the flight timetable. The implementation (Infrastructure) keeps
/// EF Core out of the Application layer and guarantees only the requested page is
/// fetched from the database.
/// </summary>
public interface IFlightRepository
{
    Task<PagedResult<Flight>> GetPagedAsync(FlightQuerySpec spec, CancellationToken cancellationToken = default);

    /// <summary>All rows matching a flight code (usually one). Empty if unknown.</summary>
    Task<IReadOnlyList<Flight>> GetByFlightCodeAsync(string flightCode, CancellationToken cancellationToken = default);
}
