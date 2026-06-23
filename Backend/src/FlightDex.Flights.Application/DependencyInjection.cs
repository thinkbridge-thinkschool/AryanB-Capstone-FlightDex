using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Flights.Application;

public static class DependencyInjection
{
    /// <summary>Registers the Flights module's query handlers.</summary>
    public static IServiceCollection AddFlightsApplication(this IServiceCollection services)
    {
        // TODO: register GetFlights / GetFlightByCode handlers.
        return services;
    }
}
