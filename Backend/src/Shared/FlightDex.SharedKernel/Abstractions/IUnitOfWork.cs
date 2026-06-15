namespace FlightDex.SharedKernel.Abstractions;

/// <summary>Transaction boundary abstraction shared by infrastructure layers.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
