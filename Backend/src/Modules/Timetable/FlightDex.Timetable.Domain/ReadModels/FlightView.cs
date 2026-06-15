namespace FlightDex.Timetable.Domain.ReadModels;

public sealed class FlightView
{
    public Guid FlightId { get; set; }
    public Guid AirlineId { get; set; }
    public string Airline { get; set; } = default!;
    public Guid RouteId { get; set; }

    public string FromAirportCode { get; set; } = default!;
    public string FromCity { get; set; } = default!;
    public string FromState { get; set; } = default!;
    public string FromCountry { get; set; } = default!;

    public string ToAirportCode { get; set; } = default!;
    public string ToCity { get; set; } = default!;
    public string ToState { get; set; } = default!;
    public string ToCountry { get; set; } = default!;

    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public string Status { get; set; } = default!;
}
