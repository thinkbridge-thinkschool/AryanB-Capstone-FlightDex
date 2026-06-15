using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

/// <summary>Detail entity within the Flight aggregate; never loaded independently.</summary>
public sealed class FlightDetails : Entity<Guid>
{
    public string AirplaneModel { get; private set; } = default!;
    public SeatingPlan Seats { get; private set; } = default!;
    public TimeSpan AverageDuration { get; private set; }

    // Parameterless ctor reserved for the ORM materializer.
    private FlightDetails() { }

    private FlightDetails(Guid id, string airplaneModel, SeatingPlan seats, TimeSpan averageDuration) : base(id)
    {
        AirplaneModel = airplaneModel;
        Seats = seats;
        AverageDuration = averageDuration;
    }

    public static FlightDetails Create(string airplaneModel, SeatingPlan seats, TimeSpan averageDuration)
        => throw new NotImplementedException();
}
