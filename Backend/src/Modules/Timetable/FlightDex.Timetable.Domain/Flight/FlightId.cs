namespace FlightDex.Timetable.Domain.Flight;

/// <summary>Strongly-typed identifier for the Flight aggregate.</summary>
public sealed record FlightId(Guid Value)
{
    public static FlightId New() => new(Guid.NewGuid());

    public static FlightId From(Guid value) => new(value);
}
