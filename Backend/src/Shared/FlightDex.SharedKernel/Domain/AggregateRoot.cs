namespace FlightDex.SharedKernel.Domain;

/// <summary>Base type for aggregate roots (identity + domain event tracking).</summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id) { }

    protected AggregateRoot() { }

    protected void Raise(IDomainEvent domainEvent) => throw new NotImplementedException();

    public void ClearDomainEvents() => throw new NotImplementedException();
}
