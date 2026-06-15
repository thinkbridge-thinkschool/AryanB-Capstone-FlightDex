using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

/// <summary>Flight aggregate root. Owns <see cref="FlightDetails"/>; references Route by identity.</summary>
public sealed class Flight : AggregateRoot<FlightId>
{
    public string Airline { get; private set; } = default!;

    /// <summary>Identity reference into the Routing context (no cross-context coupling).</summary>
    public Guid RouteId { get; private set; }

    public FlightSchedule Schedule { get; private set; } = default!;
    public FlightDetails Details { get; private set; } = default!;

    // Parameterless ctor reserved for the ORM materializer.
    private Flight() { }

    private Flight(FlightId id, string airline, Guid routeId, FlightSchedule schedule, FlightDetails details) : base(id)
    {
        Airline = airline;
        RouteId = routeId;
        Schedule = schedule;
        Details = details;
    }

    public static Flight Create(
        FlightId id,
        string airline,
        Guid routeId,
        FlightSchedule schedule,
        FlightDetails details) => throw new NotImplementedException();

    public void Reschedule(FlightSchedule schedule) => throw new NotImplementedException();

    public void UpdateDetails(FlightDetails details) => throw new NotImplementedException();
}
