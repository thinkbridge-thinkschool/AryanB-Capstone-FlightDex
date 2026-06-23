namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Marker interface for a CQRS command that returns <typeparamref name="TResult"/>.
/// Commands change state (register a user, book or cancel a ticket); queries do not.
/// </summary>
public interface ICommand<TResult>;
