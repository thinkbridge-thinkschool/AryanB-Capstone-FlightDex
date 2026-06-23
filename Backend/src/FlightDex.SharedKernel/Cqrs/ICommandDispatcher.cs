namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Resolves and invokes the handler for a command. Decouples the API layer from
/// concrete handler types so controllers depend only on this abstraction.
/// </summary>
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}
