using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Booking.Application;

public static class DependencyInjection
{
    /// <summary>Registers the Booking module's command and query handlers.</summary>
    public static IServiceCollection AddBookingApplication(this IServiceCollection services)
    {
        // TODO: register RegisterUser / Login / BookTicket / CancelTicket / GetMyTickets handlers.
        return services;
    }
}
