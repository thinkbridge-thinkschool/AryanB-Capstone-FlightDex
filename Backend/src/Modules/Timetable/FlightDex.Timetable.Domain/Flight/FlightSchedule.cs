using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

/// <summary>Value object: departure/arrival timestamps; derives flight duration.</summary>
public sealed class FlightSchedule : ValueObject
{
    public DateTime DepartureUtc { get; }
    public DateTime ArrivalUtc { get; }

    public TimeSpan Duration => throw new NotImplementedException();

    private FlightSchedule(DateTime departureUtc, DateTime arrivalUtc)
    {
        DepartureUtc = departureUtc;
        ArrivalUtc = arrivalUtc;
    }

    public static FlightSchedule Create(DateTime departureUtc, DateTime arrivalUtc) => throw new NotImplementedException();

    protected override IEnumerable<object?> GetEqualityComponents() => throw new NotImplementedException();
}
