using FlightDex.SharedKernel.Domain;

namespace FlightDex.Locations.Domain;

/// <summary>Location aggregate root: an airport identified by its <see cref="AirportCode"/>.</summary>
public sealed class Location : AggregateRoot<AirportCode>
{
    public string AirportName { get; private set; } = default!;
    public Address Address { get; private set; } = default!;

    // Parameterless ctor reserved for the ORM materializer.
    private Location() { }

    private Location(AirportCode code, string airportName, Address address) : base(code)
    {
        AirportName = airportName;
        Address = address;
    }

    public static Location Create(AirportCode code, string airportName, Address address) => throw new NotImplementedException();

    public void Rename(string airportName) => throw new NotImplementedException();

    public void Relocate(Address address) => throw new NotImplementedException();
}
