using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Timetable.Infrastructure;

/// <summary>
/// DI registration for the Timetable context: db context, repository, query/event handlers,
/// the FlightView read store, and the route/location lookup adapters.
/// </summary>
public static class TimetableModule
{
    public static IServiceCollection AddTimetableModule(
        this IServiceCollection services,
        IConfiguration configuration) => throw new NotImplementedException();
}
