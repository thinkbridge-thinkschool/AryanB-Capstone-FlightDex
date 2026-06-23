namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Resolves and invokes the handler for a query. Decouples the API layer from
/// concrete handler types so controllers depend only on this abstraction.
/// </summary>
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
