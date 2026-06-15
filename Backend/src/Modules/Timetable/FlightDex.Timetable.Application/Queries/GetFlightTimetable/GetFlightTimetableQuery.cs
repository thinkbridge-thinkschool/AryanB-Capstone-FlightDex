using FlightDex.SharedKernel.Pagination;

namespace FlightDex.Timetable.Application.Queries.GetFlightTimetable;

/// <summary>Paginated timetable query input with optional filters and sort.</summary>
public sealed record GetFlightTimetableQuery(
    PagedRequest Page,
    string? DepartureAirportCode = null,
    string? ArrivalAirportCode = null,
    string? AirportCode = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    FlightSortBy SortBy = FlightSortBy.Departure);

/// <summary>Sort options for the timetable.</summary>
public enum FlightSortBy
{
    Departure,
    Arrival,
    Airline
}
