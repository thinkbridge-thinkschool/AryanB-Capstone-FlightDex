using FlightDex.SharedKernel.Domain;

namespace FlightDex.Locations.Domain;

/// <summary>Value object: city/state/country of an airport.</summary>
public sealed class Address : ValueObject
{
    public string City { get; }
    public string State { get; }
    public string Country { get; }

    private Address(string city, string state, string country)
    {
        City = city;
        State = state;
        Country = country;
    }

    public static Address Create(string city, string state, string country) => throw new NotImplementedException();

    protected override IEnumerable<object?> GetEqualityComponents() => throw new NotImplementedException();
}
