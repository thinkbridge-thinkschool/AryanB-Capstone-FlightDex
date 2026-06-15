using FlightDex.IntegrationEvents;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Application.EventHandlers;

public sealed class RouteChangedHandler
{
    private readonly FlightViewProjector _projector;

    public RouteChangedHandler(FlightViewProjector projector) => _projector = projector;

    public Task HandleAsync(RouteChangedEvent integrationEvent, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
