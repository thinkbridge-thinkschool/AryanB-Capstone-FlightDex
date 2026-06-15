using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

public sealed class FlightDetails : Entity<Guid>
{
    public string AirplaneModel { get; private set; } = default!;
    public SeatingPlan Seats { get; private set; } = default!;
    public TimeSpan AverageDuration { get; private set; }

    private FlightDetails() { }

    private FlightDetails(Guid id, string airplaneModel, SeatingPlan seats, TimeSpan averageDuration)
        : base(id)
    {
        AirplaneModel = airplaneModel;
        Seats = seats;
        AverageDuration = averageDuration;
    }

    public static FlightDetails Create(string airplaneModel, SeatingPlan seats, TimeSpan averageDuration)
        => new(Guid.NewGuid(), airplaneModel, seats, averageDuration);
}
