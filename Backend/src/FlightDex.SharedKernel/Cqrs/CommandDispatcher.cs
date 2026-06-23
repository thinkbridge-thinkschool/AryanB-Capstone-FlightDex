using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Default dispatcher: resolves the closed-generic handler from the container.</summary>
internal sealed class CommandDispatcher(IServiceProvider provider) : ICommandDispatcher
{
    public Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        // TODO: resolve ICommandHandler<TCommand, TResult> for command.GetType() and invoke it.
        throw new NotImplementedException();
    }
}
