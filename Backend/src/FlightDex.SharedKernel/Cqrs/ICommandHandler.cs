namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Handles a single <see cref="ICommand{TResult}"/> type.</summary>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
