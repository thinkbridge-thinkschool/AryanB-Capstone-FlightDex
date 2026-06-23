using FlightDex.Booking.Domain;

namespace FlightDex.Booking.Application.Dtos;

/// <summary>
/// A booked ticket as shown in "My Tickets". Date is "yyyy-MM-dd" and Time is "HH:mm"
/// so the client never has to reparse a locale-specific date.
/// </summary>
public sealed record TicketDto(
    int TicketId,
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
    public static TicketDto FromDomain(Ticket t) => new(
        t.Id,
        t.Date.ToString("yyyy-MM-dd"),
        t.Time.ToString("HH\\:mm"),
        t.OriginCode,
        t.OriginAirport,
        t.OriginCity,
        t.DestinationCode,
        t.DestinationAirport,
        t.DestinationCity,
        t.FirstName,
        t.LastName,
        t.Age);
}
