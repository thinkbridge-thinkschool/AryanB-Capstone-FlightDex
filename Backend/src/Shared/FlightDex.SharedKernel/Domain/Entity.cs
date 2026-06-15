namespace FlightDex.SharedKernel.Domain;

/// <summary>Base type for entities, compared by strongly-typed identity.</summary>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    protected Entity(TId id) => Id = id;

    // Parameterless ctor reserved for the ORM materializer.
    protected Entity() { }

    public bool Equals(Entity<TId>? other) => throw new NotImplementedException();

    public override bool Equals(object? obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}
