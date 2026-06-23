using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Resolves the <see cref="ICommandHandler{TCommand,TResult}"/> for the runtime command
/// type from the DI container and invokes it. Registered as scoped.
/// </summary>
internal sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = serviceProvider.GetRequiredService(handlerType);

        // HandleAsync(command, ct) — invoked via reflection because the concrete command
        // type is only known at runtime.
        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResult>, TResult>.HandleAsync))!;
        return (Task<TResult>)method.Invoke(handler, [command, cancellationToken])!;
    }
}
