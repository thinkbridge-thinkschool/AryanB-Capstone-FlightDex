using FlightDex.SharedKernel.Domain;

namespace FlightDex.Timetable.Domain.Airline;

public sealed class Airline : AggregateRoot<AirlineId>
{
    public string Name { get; private set; } = default!;

    private Airline() { }

    private Airline(AirlineId id, string name) : base(id)
    {
        Name = name;
    }

    public static Airline Create(AirlineId id, string name) => new(id, name);
}
