namespace FlightDex.Flights.Domain;

/// <summary>
/// A single recurring (daily) timetable entry for one of the served airports
/// (BLR, BOM, PNQ, LON, DBX). The model unifies departures and arrivals: <see cref="Direction"/>
/// says which it is, and the "Counterpart" fields describe the <em>other</em> end of
/// the leg — the destination for a departure, the origin for an arrival.
/// </summary>
public sealed class Flight
{
    public int Id { get; private set; }

    /// <summary>The served airport this row belongs to: BLR, BOM, PNQ, LON or DBX.</summary>
    public string Airport { get; private set; } = string.Empty;

    public FlightDirection Direction { get; private set; }

    /// <summary>Departure time (for a departure) or arrival time (for an arrival).</summary>
    public TimeOnly ScheduledTime { get; private set; }

    public string AirlineCode { get; private set; } = string.Empty;
    public string Airline { get; private set; } = string.Empty;

    /// <summary>Full name of the destination/origin airport.</summary>
    public string CounterpartAirport { get; private set; } = string.Empty;

    /// <summary>IATA code of the destination (departure) or origin (arrival).</summary>
    public string CounterpartCode { get; private set; } = string.Empty;

    /// <summary>City of the destination (departure) or origin (arrival).</summary>
    public string CounterpartCity { get; private set; } = string.Empty;

    public string FlightCode { get; private set; } = string.Empty;

    /// <summary>Block time as printed on the timetable, e.g. "1h 35m".</summary>
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
        Airport = airport;
        Direction = direction;
        ScheduledTime = scheduledTime;
        AirlineCode = airlineCode;
        Airline = airline;
        CounterpartAirport = counterpartAirport;
        CounterpartCode = counterpartCode;
        CounterpartCity = counterpartCity;
        FlightCode = flightCode;
        Duration = duration;
    }
}
