namespace FlightDex.SharedKernel.Cqrs;

/// <summary>Marker for a command that returns a result.</summary>
public interface ICommand<TResult>;
