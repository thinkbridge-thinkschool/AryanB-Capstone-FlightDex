using FlightDex.Flights.Domain;

namespace FlightDex.Flights.Application.Dtos;

/// <summary>
/// The compact row shown in a departures/arrivals list: time, airline, flight code
/// and the destination/origin city. Time is rendered "HH:mm".
/// </summary>
public sealed record FlightListItem(
    string FlightCode,
    string Time,
    string Airline,
    string City,
    string Code,
    string Airport,
    FlightDirection Direction)
{
    public static FlightListItem FromDomain(Flight f) => new(
        f.FlightCode,
        f.ScheduledTime.ToString("HH\\:mm"),
        f.Airline,
        f.CounterpartCity,
        f.CounterpartCode,
        f.CounterpartAirport,
        f.Direction);
}
