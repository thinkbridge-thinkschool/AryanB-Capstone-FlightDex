using FlightDex.Flights.Application.Dtos;
using FlightDex.Flights.Application.Queries.GetFlightByCode;
using FlightDex.Flights.Application.Queries.GetFlights;
using FlightDex.SharedKernel.Cqrs;
using FlightDex.SharedKernel.Paging;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Flights.Application;

public static class DependencyInjection
{
    /// <summary>Registers the Flights module's CQRS query handlers.</summary>
    public static IServiceCollection AddFlightsApplication(this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetFlightsQuery, PagedResult<FlightListItem>>,
            GetFlightsQueryHandler>();

        services.AddScoped<
            IQueryHandler<GetFlightByCodeQuery, IReadOnlyList<FlightDetail>>,
            GetFlightByCodeQueryHandler>();

        return services;
    }
}
