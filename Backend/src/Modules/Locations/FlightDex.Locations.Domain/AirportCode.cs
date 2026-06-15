using FlightDex.SharedKernel.Domain;

namespace FlightDex.Locations.Domain;

/// <summary>Strongly-typed airport (IATA) code; identity of the Location aggregate.</summary>
public sealed class AirportCode : ValueObject
{
    public string Value { get; }

    private AirportCode(string value) => Value = value;

    public static AirportCode Create(string value) => new(value.ToUpperInvariant());

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value;
}
