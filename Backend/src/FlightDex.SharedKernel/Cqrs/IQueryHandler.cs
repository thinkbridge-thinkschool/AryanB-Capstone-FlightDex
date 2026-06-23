namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Handles a single <see cref="IQuery{TResult}"/> type.</summary>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
