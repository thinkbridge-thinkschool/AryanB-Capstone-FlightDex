namespace FlightDex.SharedKernel.Domain;

/// <summary>Base type for value objects, compared by structural equality.</summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Components that participate in equality, in order.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other) => throw new NotImplementedException();

    public override bool Equals(object? obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}
