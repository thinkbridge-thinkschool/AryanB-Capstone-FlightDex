namespace FlightDex.Flights.Domain;

/// <summary>
/// A single recurring (daily) timetable entry for one of the served airports (BLR, BOM, PNQ).
/// The model unifies departures and arrivals: <see cref="Direction"/> says which it is, and the
/// "Counterpart" fields describe the other end of the leg.
/// </summary>
public sealed class Flight
{
    public int Id { get; private set; }
    public string Airport { get; private set; } = string.Empty;
    public FlightDirection Direction { get; private set; }
    public TimeOnly ScheduledTime { get; private set; }
    public string AirlineCode { get; private set; } = string.Empty;
    public string Airline { get; private set; } = string.Empty;
    public string CounterpartAirport { get; private set; } = string.Empty;
    public string CounterpartCode { get; private set; } = string.Empty;
    public string CounterpartCity { get; private set; } = string.Empty;
    public string FlightCode { get; private set; } = string.Empty;
    public string Duration { get; private set; } = string.Empty;

    private Flight() { } // EF Core

    public Flight(
        string airport,
        FlightDirection direction,
        TimeOnly scheduledTime,
        string airlineCode,
        string airline,
        string counterpartAirport,
        string counterpartCode,
        string counterpartCity,
        string flightCode,
        string duration)
    {
        // TODO: assign fields (and any invariant checks).
        throw new NotImplementedException();
    }
}
