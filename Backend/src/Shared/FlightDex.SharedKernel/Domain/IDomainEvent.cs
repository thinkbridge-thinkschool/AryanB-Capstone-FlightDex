namespace FlightDex.SharedKernel.Domain;

/// <summary>Marker for domain events raised within an aggregate boundary.</summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
