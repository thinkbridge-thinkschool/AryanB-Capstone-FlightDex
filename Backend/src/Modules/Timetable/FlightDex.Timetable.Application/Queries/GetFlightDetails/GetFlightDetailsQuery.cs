namespace FlightDex.Timetable.Application.Queries.GetFlightDetails;

/// <summary>Single-flight detail query input (flight id).</summary>
public sealed record GetFlightDetailsQuery(Guid FlightId);
