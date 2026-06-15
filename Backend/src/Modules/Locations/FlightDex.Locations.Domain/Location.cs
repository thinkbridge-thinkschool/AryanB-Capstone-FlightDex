using FlightDex.SharedKernel.Domain;

namespace FlightDex.Locations.Domain;

public sealed class Location : AggregateRoot<AirportCode>
{
    public Address Address { get; private set; } = default!;

    private Location() { }

    private Location(AirportCode code, Address address) : base(code)
    {
        Address = address;
    }

    public static Location Create(AirportCode code, Address address)
        => new(code, address);

    public void Relocate(Address address) => Address = address;
}
