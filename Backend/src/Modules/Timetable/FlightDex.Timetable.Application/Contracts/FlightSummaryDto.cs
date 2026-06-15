namespace FlightDex.Timetable.Application.Contracts;

public sealed record FlightSummaryDto(
    Guid FlightId,
    string Airline,
    string FromCity,
    string ToCity,
    DateTime DepartureUtc,
    DateTime ArrivalUtc,
    string Status);
