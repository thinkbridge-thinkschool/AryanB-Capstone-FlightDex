using FlightDex.Locations.Domain;

namespace FlightDex.Locations.Infrastructure.Persistence;

/// <summary><see cref="ILocationRepository"/> implementation backed by EF Core.</summary>
public sealed class LocationRepository : ILocationRepository
{
    private readonly LocationsDbContext _dbContext;

    public LocationRepository(LocationsDbContext dbContext) => _dbContext = dbContext;

    public Task<Location?> GetByCodeAsync(AirportCode code, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddAsync(Location location, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public void Update(Location location) => throw new NotImplementedException();
}
