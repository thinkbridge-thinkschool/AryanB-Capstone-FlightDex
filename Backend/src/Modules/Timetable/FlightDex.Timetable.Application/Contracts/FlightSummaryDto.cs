namespace FlightDex.Timetable.Application.Contracts;

/// <summary>Timetable row shape returned by the paginated timetable query.</summary>
public sealed record FlightSummaryDto(
    Guid FlightId,
    string Airline,
    string FromAirportCode,
    string FromCity,
    string ToAirportCode,
    string ToCity,
    DateTime DepartureUtc,
    DateTime ArrivalUtc);
