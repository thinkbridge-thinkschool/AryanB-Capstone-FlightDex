namespace FlightDex.Routing.Domain;

/// <summary>Persistence port for the Route aggregate.</summary>
public interface IRouteRepository
{
    Task<Route?> GetByIdAsync(RouteId id, CancellationToken cancellationToken = default);

    Task AddAsync(Route route, CancellationToken cancellationToken = default);

    void Update(Route route);
}
