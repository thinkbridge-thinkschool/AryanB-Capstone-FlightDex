using FlightDex.Routing.Domain;

namespace FlightDex.Routing.Infrastructure.Persistence;

/// <summary><see cref="IRouteRepository"/> implementation backed by EF Core.</summary>
public sealed class RouteRepository : IRouteRepository
{
    private readonly RoutingDbContext _dbContext;

    public RouteRepository(RoutingDbContext dbContext) => _dbContext = dbContext;

    public Task<Route?> GetByIdAsync(RouteId id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AddAsync(Route route, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public void Update(Route route) => throw new NotImplementedException();
}
