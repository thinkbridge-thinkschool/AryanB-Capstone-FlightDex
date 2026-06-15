using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Flight;

public sealed class SeatingPlan : ValueObject
{
    public int Economy { get; }
    public int Business { get; }
    public int First { get; }

    public int TotalSeats => Economy + Business + First;

    private SeatingPlan(int economy, int business, int first)
    {
        Economy = economy;
        Business = business;
        First = first;
    }

    public static SeatingPlan Create(int economy, int business, int first)
        => new(economy, business, first);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Economy;
        yield return Business;
        yield return First;
    }
}
