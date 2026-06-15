using FlightDex.Locations.Application.Contracts;
using FlightDex.Locations.Domain;

namespace FlightDex.Locations.Application.Queries.GetLocationByCode;

public sealed class GetLocationByCodeHandler
{
    private readonly ILocationRepository _repository;

    public GetLocationByCodeHandler(ILocationRepository repository) => _repository = repository;

    public async Task<LocationDto?> HandleAsync(GetLocationByCodeQuery query, CancellationToken cancellationToken = default)
    {
        var code = AirportCode.Create(query.AirportCode);
        var location = await _repository.GetByCodeAsync(code, cancellationToken);
        if (location is null) return null;
        return new LocationDto(
            location.Id.Value,
            location.Address.City,
            location.Address.State,
            location.Address.Country);
    }
}
