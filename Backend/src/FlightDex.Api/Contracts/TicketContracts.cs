namespace FlightDex.Api.Contracts;

/// <summary>An origin/destination airport as sent by the client when booking.</summary>
public sealed record AirportRef(string Code, string Airport, string City);

/// <summary>
/// Booking payload. Date is "yyyy-MM-dd", Time is "HH:mm". The passenger's name and age
/// are taken from the authenticated user, not this request.
/// </summary>
public sealed record BookTicketRequest(
    string Date,
    string Time,
    AirportRef Origin,
    AirportRef Destination);
