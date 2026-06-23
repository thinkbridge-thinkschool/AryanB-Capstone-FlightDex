using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Default dispatcher: resolves the closed-generic handler from the container.</summary>
internal sealed class QueryDispatcher(IServiceProvider provider) : IQueryDispatcher
{
    public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        // TODO: resolve IQueryHandler<TQuery, TResult> for query.GetType() and invoke it.
        throw new NotImplementedException();
    }
}
