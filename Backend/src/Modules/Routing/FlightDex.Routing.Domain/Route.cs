using FlightDex.SharedKernel.Domain;

namespace FlightDex.Routing.Domain;

/// <summary>Route aggregate root: maps a <see cref="RouteId"/> to a from/to airport pair.</summary>
public sealed class Route : AggregateRoot<RouteId>
{
    public AirportCode From { get; private set; } = default!;
    public AirportCode To { get; private set; } = default!;

    // Parameterless ctor reserved for the ORM materializer.
    private Route() { }

    private Route(RouteId id, AirportCode from, AirportCode to) : base(id)
    {
        From = from;
        To = to;
    }

    public static Route Create(RouteId id, AirportCode from, AirportCode to) => throw new NotImplementedException();

    public void ChangeEndpoints(AirportCode from, AirportCode to) => throw new NotImplementedException();
}
