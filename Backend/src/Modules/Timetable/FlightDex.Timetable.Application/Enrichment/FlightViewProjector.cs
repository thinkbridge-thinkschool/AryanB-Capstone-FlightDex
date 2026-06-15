using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Domain.ReadModels;

namespace FlightDex.Timetable.Application.Enrichment;

/// <summary>Builds/refreshes a <see cref="FlightView"/> from flight + route + location data.</summary>
public sealed class FlightViewProjector
{
    private readonly IRouteLookup _routeLookup;
    private readonly ILocationLookup _locationLookup;

    public FlightViewProjector(IRouteLookup routeLookup, ILocationLookup locationLookup)
    {
        _routeLookup = routeLookup;
        _locationLookup = locationLookup;
    }

    /// <summary>Projects a single flight into a denormalized read-model row.</summary>
    public Task<FlightView> ProjectAsync(Flight flight, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
