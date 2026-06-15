using FlightDex.IntegrationEvents;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Application.EventHandlers;

/// <summary>Refreshes FlightViews referencing an airport code when <see cref="LocationChangedEvent"/> is published.</summary>
public sealed class LocationChangedHandler
{
    private readonly FlightViewProjector _projector;

    public LocationChangedHandler(FlightViewProjector projector) => _projector = projector;

    public Task HandleAsync(LocationChangedEvent integrationEvent, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
