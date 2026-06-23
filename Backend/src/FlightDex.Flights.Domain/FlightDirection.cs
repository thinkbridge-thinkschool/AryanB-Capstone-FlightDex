namespace FlightDex.Flights.Domain;

/// <summary>
/// Whether a timetable row describes a flight leaving an airport or arriving at it.
/// </summary>
public enum FlightDirection
{
    Departure = 0,
    Arrival = 1,
}
