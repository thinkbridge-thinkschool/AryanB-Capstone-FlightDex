namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Resolves and invokes the handler for a command.</summary>
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}
