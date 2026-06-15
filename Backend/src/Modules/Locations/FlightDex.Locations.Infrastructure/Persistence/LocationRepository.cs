using FlightDex.Locations.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Locations.Infrastructure.Persistence;

public sealed class LocationRepository : ILocationRepository
{
    private readonly LocationsDbContext _dbContext;

    public LocationRepository(LocationsDbContext dbContext) => _dbContext = dbContext;

    public Task<Location?> GetByCodeAsync(AirportCode code, CancellationToken cancellationToken = default)
    {
        var val = code.Value;
        return _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Id == AirportCode.Create(val), cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
        => await _dbContext.Locations.AddAsync(location, cancellationToken);

    public void Update(Location location)
        => _dbContext.Locations.Update(location);
}
