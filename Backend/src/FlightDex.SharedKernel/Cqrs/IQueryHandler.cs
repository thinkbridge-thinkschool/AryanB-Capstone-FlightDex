namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Handles a single query type. One handler per query keeps the read side thin.
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
