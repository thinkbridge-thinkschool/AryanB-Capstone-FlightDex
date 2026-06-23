using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Resolves the <see cref="IQueryHandler{TQuery,TResult}"/> for the runtime query type
/// from the DI container and invokes it. Registered as scoped.
/// </summary>
internal sealed class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = serviceProvider.GetRequiredService(handlerType);

        // HandleAsync(query, ct) — invoked via reflection because the concrete query type
        // is only known at runtime.
        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))!;
        return (Task<TResult>)method.Invoke(handler, [query, cancellationToken])!;
    }
}
