using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Flights.Application.Queries.GetFlightByCode;

/// <summary>Looks up the leg(s) for a flight code.</summary>
public sealed record GetFlightByCodeQuery(string FlightCode) : IQuery<IReadOnlyList<FlightDetail>>;
