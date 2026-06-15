namespace FlightDex.Timetable.Application.Contracts;

/// <summary>Expanded detail-panel shape for a single flight.</summary>
public sealed record FlightDetailsDto(
    Guid FlightId,
    string Airline,
    // Origin.
    string FromAirportCode,
    string FromAirportName,
    string FromCity,
    string FromState,
    string FromCountry,
    // Destination.
    string ToAirportCode,
    string ToAirportName,
    string ToCity,
    string ToState,
    string ToCountry,
    // Schedule.
    DateTime DepartureUtc,
    DateTime ArrivalUtc,
    TimeSpan Duration,
    // Details.
    string AirplaneModel,
    int EconomySeats,
    int BusinessSeats,
    int FirstSeats,
    TimeSpan AverageDuration);
