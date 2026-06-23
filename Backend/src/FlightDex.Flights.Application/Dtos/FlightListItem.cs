using FlightDex.Flights.Domain;

namespace FlightDex.Flights.Application.Dtos;

/// <summary>The compact row shown in a departures/arrivals list. Time is rendered "HH:mm".</summary>
public sealed record FlightListItem(
    string FlightCode,
    string Time,
    string Airline,
    string City,
    string Code,
    string Airport,
    FlightDirection Direction)
{
    public static FlightListItem FromDomain(Flight f) =>
        throw new NotImplementedException(); // TODO: map from the Flight entity.
}
