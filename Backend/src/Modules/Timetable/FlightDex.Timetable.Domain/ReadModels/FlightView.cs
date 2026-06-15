namespace FlightDex.Timetable.Domain.ReadModels;

/// <summary>Denormalized read model backing the dashboard timetable and detail panel.</summary>
public sealed class FlightView
{
    public Guid FlightId { get; set; }
    public string Airline { get; set; } = default!;
    public Guid RouteId { get; set; }

    // Origin (flattened from Route + Location).
    public string FromAirportCode { get; set; } = default!;
    public string FromAirportName { get; set; } = default!;
    public string FromCity { get; set; } = default!;
    public string FromState { get; set; } = default!;
    public string FromCountry { get; set; } = default!;

    // Destination (flattened from Route + Location).
    public string ToAirportCode { get; set; } = default!;
    public string ToAirportName { get; set; } = default!;
    public string ToCity { get; set; } = default!;
    public string ToState { get; set; } = default!;
    public string ToCountry { get; set; } = default!;

    // Schedule.
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }

    // Details.
    public string AirplaneModel { get; set; } = default!;
    public int EconomySeats { get; set; }
    public int BusinessSeats { get; set; }
    public int FirstSeats { get; set; }
    public TimeSpan AverageDuration { get; set; }
}
