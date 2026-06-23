using FlightDex.Flights.Domain;

namespace FlightDex.Flights.Application.Dtos;

/// <summary>
/// Every column for a flight — returned by GET /flight/{flightCode} (the "click to
/// view details" payload). Direction-specific naming is exposed so the client can
/// label the counterpart as destination or origin.
/// </summary>
public sealed record FlightDetail(
    string FlightCode,
    string Airport,
    FlightDirection Direction,
    string Time,
    string AirlineCode,
    string Airline,
    string CounterpartAirport,
    string CounterpartCode,
    string CounterpartCity,
    string Duration)
{
    public static FlightDetail FromDomain(Flight f) => new(
        f.FlightCode,
        f.Airport,
        f.Direction,
        f.ScheduledTime.ToString("HH\\:mm"),
        f.AirlineCode,
        f.Airline,
        f.CounterpartAirport,
        f.CounterpartCode,
        f.CounterpartCity,
        f.Duration);
}
