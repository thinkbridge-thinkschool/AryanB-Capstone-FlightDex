namespace FlightDex.Timetable.Application.Contracts;

public sealed record FlightDetailsDto(
    Guid FlightId,
    string Airline,
    string FromAirportCode,
    string FromCity,
    string FromState,
    string FromCountry,
    string ToAirportCode,
    string ToCity,
    string ToState,
    string ToCountry,
    DateTime DepartureUtc,
    DateTime ArrivalUtc,
    string Status);
