using FlightDex.Flights.Domain;

namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// The fully-resolved filter for a timetable query: direction, served airport, an optional
/// counterpart term (code or city), an optional time window, and paging.
/// </summary>
public sealed record FlightQuerySpec(
    FlightDirection Direction,
    string? Airport,
    string? CounterpartTerm,
    TimeOnly? TimeAfter,
    TimeOnly? TimeBefore,
    int Page,
    int PageSize);
