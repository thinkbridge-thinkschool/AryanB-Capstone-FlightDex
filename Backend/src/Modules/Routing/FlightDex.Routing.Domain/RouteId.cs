namespace FlightDex.Routing.Domain;

/// <summary>Strongly-typed identifier for the Route aggregate.</summary>
public sealed record RouteId(Guid Value)
{
    public static RouteId New() => throw new NotImplementedException();

    public static RouteId From(Guid value) => new(value);
}
