namespace FlightDex.SharedKernel.Domain;

/// <summary>Base type for value objects, compared by structural equality.</summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Components that participate in equality, in order.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
        => other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj) => obj is ValueObject vo && Equals(vo);

    public override int GetHashCode()
        => GetEqualityComponents().Select(x => x?.GetHashCode() ?? 0).Aggregate((a, b) => a ^ b);
}
