using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Application.Queries.GetFlights;

/// <summary>
/// A paged list of departures or arrivals for the timetable view. The controller maps
/// the raw query string (at / to / from / deptTime_* / arrTime_*) onto this query.
/// </summary>
public sealed record GetFlightsQuery(FlightQuerySpec Spec) : IQuery<PagedResult<FlightListItem>>;
