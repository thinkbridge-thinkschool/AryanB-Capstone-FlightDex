using FlightDex.Routing.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightDex.Routing.Infrastructure.Persistence;

public sealed class RouteRepository : IRouteRepository
{
    private readonly RoutingDbContext _dbContext;

    public RouteRepository(RoutingDbContext dbContext) => _dbContext = dbContext;

    public Task<Route?> GetByIdAsync(RouteId id, CancellationToken cancellationToken = default)
    {
        var val = id.Value;
        return _dbContext.Routes.FirstOrDefaultAsync(r => r.Id == RouteId.From(val), cancellationToken);
    }

    public async Task AddAsync(Route route, CancellationToken cancellationToken = default)
        => await _dbContext.Routes.AddAsync(route, cancellationToken);

    public void Update(Route route)
        => _dbContext.Routes.Update(route);
}
