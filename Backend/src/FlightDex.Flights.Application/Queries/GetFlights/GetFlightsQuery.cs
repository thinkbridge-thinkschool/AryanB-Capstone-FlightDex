using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Application.Queries.GetFlights;

/// <summary>A paged timetable query for departures or arrivals.</summary>
public sealed record GetFlightsQuery(FlightQuerySpec Spec) : IQuery<PagedResult<FlightListItem>>;
