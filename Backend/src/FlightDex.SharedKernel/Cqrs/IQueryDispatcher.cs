namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Resolves and invokes the handler for a query.</summary>
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
