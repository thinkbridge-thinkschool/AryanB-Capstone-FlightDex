using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Dtos;

/// <summary>The shape of a booked ticket returned to the client.</summary>
public sealed record TicketDto(
    int Id,
    string Date,
    string Time,
    string OriginCode,
    string OriginAirport,
    string OriginCity,
    string DestinationCode,
    string DestinationAirport,
    string DestinationCity,
    string FirstName,
    string LastName,
    int Age)
{
    public static TicketDto FromDomain(Ticket t) =>
        throw new NotImplementedException(); // TODO: map from the Ticket entity.
}
