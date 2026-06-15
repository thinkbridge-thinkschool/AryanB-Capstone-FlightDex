using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

public sealed class FlightSchedule : ValueObject
{
    public DateTime DepartureUtc { get; }
    public DateTime ArrivalUtc { get; }

    public TimeSpan Duration => ArrivalUtc - DepartureUtc;

    private FlightSchedule(DateTime departureUtc, DateTime arrivalUtc)
    {
        DepartureUtc = departureUtc;
        ArrivalUtc = arrivalUtc;
    }

    public static FlightSchedule Create(DateTime departureUtc, DateTime arrivalUtc)
    {
        if (arrivalUtc <= departureUtc)
            throw new ArgumentException("Arrival must be after departure.");
        return new(departureUtc, arrivalUtc);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DepartureUtc;
        yield return ArrivalUtc;
    }
}
