using FlightDex.IntegrationEvents;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Application.EventHandlers;

/// <summary>Refreshes FlightViews referencing a route when <see cref="RouteChangedEvent"/> is published.</summary>
public sealed class RouteChangedHandler
{
    private readonly FlightViewProjector _projector;

    public RouteChangedHandler(FlightViewProjector projector) => _projector = projector;

    public Task HandleAsync(RouteChangedEvent integrationEvent, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
