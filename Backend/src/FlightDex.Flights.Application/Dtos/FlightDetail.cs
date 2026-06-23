using FlightDex.Flights.Domain;

namespace FlightDex.Flights.Application.Dtos;

/// <summary>Full details for a single flight (the "click a flight" view).</summary>
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
    public static FlightDetail FromDomain(Flight f) =>
        throw new NotImplementedException(); // TODO: map from the Flight entity.
}
