namespace FlightDex.Api.Contracts;

/// <summary>One end of a leg (origin or destination) in a booking request.</summary>
public sealed record AirportRef(string Code, string Airport, string City);

/// <summary>Request body for POST /ticket.</summary>
public sealed record BookTicketRequest(
    string Date,
    string Time,
    AirportRef? Origin,
    AirportRef? Destination);
