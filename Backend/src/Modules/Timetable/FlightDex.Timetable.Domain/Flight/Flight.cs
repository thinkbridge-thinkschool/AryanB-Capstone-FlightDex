using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

public sealed class Flight : AggregateRoot<FlightId>
{
    public Guid AirlineId { get; private set; }
    public Guid RouteId { get; private set; }
    public FlightSchedule Schedule { get; private set; } = default!;
    public string Status { get; private set; } = default!;

    private Flight() { }

    private Flight(FlightId id, Guid airlineId, Guid routeId, FlightSchedule schedule, string status)
        : base(id)
    {
        AirlineId = airlineId;
        RouteId = routeId;
        Schedule = schedule;
        Status = status;
    }

    public static Flight Create(FlightId id, Guid airlineId, Guid routeId, FlightSchedule schedule, string status)
        => new(id, airlineId, routeId, schedule, status);

    public void Reschedule(FlightSchedule schedule) => Schedule = schedule;

    public void UpdateStatus(string status) => Status = status;
}
