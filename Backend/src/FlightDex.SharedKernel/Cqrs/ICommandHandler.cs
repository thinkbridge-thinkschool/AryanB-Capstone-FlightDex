namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Handles a single command type. One handler per command keeps the write side thin.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
