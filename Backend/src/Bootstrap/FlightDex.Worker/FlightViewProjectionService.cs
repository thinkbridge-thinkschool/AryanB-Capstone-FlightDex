using FlightDex.Timetable.Application.Enrichment;
using FlightDex.Timetable.Domain.Airline;
using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Worker;

/// <summary>
/// Rebuilds the FlightView read model for a single flight and writes it to SQL.
/// Every DB call here (load flight, resolve route/location, upsert view) surfaces as a
/// SQL dependency span under the worker's consume operation — the "→ DB" leg of the trace.
/// </summary>
public sealed class FlightViewProjectionService
{
    private readonly IFlightRepository _flights;
    private readonly FlightViewProjector _projector;
    private readonly TimetableDbContext _db;
    private readonly ILogger<FlightViewProjectionService> _logger;

    public FlightViewProjectionService(
        IFlightRepository flights,
        FlightViewProjector projector,
        TimetableDbContext db,
        ILogger<FlightViewProjectionService> logger)
    {
        _flights = flights;
        _projector = projector;
        _db = db;
        _logger = logger;
    }

    public async Task RebuildAsync(Guid flightId, CancellationToken cancellationToken = default)
    {
        var flight = await _flights.GetByIdAsync(FlightId.From(flightId), cancellationToken);
        if (flight is null)
        {
            _logger.LogWarning("Flight {FlightId} not found; nothing to project", flightId);
            return;
        }

        var airline = await _db.Airlines
            .FirstOrDefaultAsync(a => a.Id == AirlineId.From(flight.AirlineId), cancellationToken);

        var view = await _projector.ProjectAsync(flight, airline?.Name ?? "Unknown", cancellationToken);
        if (view is null)
        {
            _logger.LogWarning("Could not project FlightView for {FlightId} (missing route or location)", flightId);
            return;
        }

        var existing = await _db.FlightViews.FirstOrDefaultAsync(v => v.FlightId == flightId, cancellationToken);
        if (existing is null)
            _db.FlightViews.Add(view);
        else
            _db.Entry(existing).CurrentValues.SetValues(view);

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Projected FlightView for {FlightId}", flightId);
    }
}
