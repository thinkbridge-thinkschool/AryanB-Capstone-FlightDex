using FlightDex.Locations.Application.Contracts;
using FlightDex.Locations.Domain;

namespace FlightDex.Locations.Application.Queries.GetLocationByCode;

/// <summary>Returns airport name and city/state/country for a given airport code.</summary>
public sealed class GetLocationByCodeHandler
{
    private readonly ILocationRepository _repository;

    public GetLocationByCodeHandler(ILocationRepository repository) => _repository = repository;

    public Task<LocationDto?> HandleAsync(GetLocationByCodeQuery query, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
