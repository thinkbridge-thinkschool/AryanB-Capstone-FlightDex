using FlightDex.IntegrationEvents;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Application.EventHandlers;

public sealed class LocationChangedHandler
{
    private readonly FlightViewProjector _projector;

    public LocationChangedHandler(FlightViewProjector projector) => _projector = projector;

    public Task HandleAsync(LocationChangedEvent integrationEvent, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
