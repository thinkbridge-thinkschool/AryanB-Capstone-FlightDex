namespace FlightDex.Timetable.Domain.Airline;

public sealed record AirlineId(Guid Value)
{
    public static AirlineId New() => new(Guid.NewGuid());
    public static AirlineId From(Guid value) => new(value);
}
