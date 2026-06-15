namespace FlightDex.Locations.Domain;

/// <summary>Persistence port for the Location aggregate.</summary>
public interface ILocationRepository
{
    Task<Location?> GetByCodeAsync(AirportCode code, CancellationToken cancellationToken = default);

    Task AddAsync(Location location, CancellationToken cancellationToken = default);

    void Update(Location location);
}
