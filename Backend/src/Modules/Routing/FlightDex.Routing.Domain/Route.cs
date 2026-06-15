using FlightDex.SharedKernel.Domain;

namespace FlightDex.Routing.Domain;

public sealed class Route : AggregateRoot<RouteId>
{
    public AirportCode From { get; private set; } = default!;
    public AirportCode To { get; private set; } = default!;

    private Route() { }

    private Route(RouteId id, AirportCode from, AirportCode to) : base(id)
    {
        From = from;
        To = to;
    }

    public static Route Create(RouteId id, AirportCode from, AirportCode to)
        => new(id, from, to);

    public void ChangeEndpoints(AirportCode from, AirportCode to)
    {
        From = from;
        To = to;
    }
}
