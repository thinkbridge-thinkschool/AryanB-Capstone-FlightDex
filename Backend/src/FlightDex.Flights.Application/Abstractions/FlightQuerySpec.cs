using FlightDex.Flights.Domain;

namespace FlightDex.Flights.Application.Abstractions;

/// <summary>
/// The set of filters that shape a paged flight list. Built by the query handler and
/// translated to SQL by the repository. All filters are optional except direction and
/// paging; results are always sorted by <see cref="Flight.ScheduledTime"/> ascending.
/// </summary>
public sealed record FlightQuerySpec(
    FlightDirection Direction,
    string? Airport,
    string? CounterpartTerm,
    TimeOnly? TimeAfter,
    TimeOnly? TimeBefore,
    int Page,
    int PageSize);
