using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Flights.Application.Queries.GetFlightByCode;

/// <summary>
/// Full details for a flight code (GET /flight/{flightCode}). Returns every matching
/// row — typically one, but a code can appear as both a departure and an arrival across
/// the served airports.
/// </summary>
public sealed record GetFlightByCodeQuery(string FlightCode) : IQuery<IReadOnlyList<FlightDetail>>;
