using FlightDex.SharedKernel.Domain;

namespace FlightDex.Routing.Domain;

/// <summary>Value object for an airport (IATA) code referenced by a route endpoint.</summary>
public sealed class AirportCode : ValueObject
{
    public string Value { get; }

    private AirportCode(string value) => Value = value;

    public static AirportCode Create(string value) => new(value.ToUpperInvariant());

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value;
}
