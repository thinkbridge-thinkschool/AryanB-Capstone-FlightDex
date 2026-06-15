using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

/// <summary>Value object: seat counts by class.</summary>
public sealed class SeatingPlan : ValueObject
{
    public int Economy { get; }
    public int Business { get; }
    public int First { get; }

    public int TotalSeats => throw new NotImplementedException();

    private SeatingPlan(int economy, int business, int first)
    {
        Economy = economy;
        Business = business;
        First = first;
    }

    public static SeatingPlan Create(int economy, int business, int first) => throw new NotImplementedException();

    protected override IEnumerable<object?> GetEqualityComponents() => throw new NotImplementedException();
}
