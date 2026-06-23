namespace FlightDex.SharedKernel.Cqrs;

/// <summary>
/// Marker interface for a CQRS query that returns <typeparamref name="TResult"/>.
/// Queries are read-only and must never mutate state.
/// </summary>
public interface IQuery<TResult>;
