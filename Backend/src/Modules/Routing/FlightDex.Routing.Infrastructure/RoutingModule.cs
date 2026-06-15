using FlightDex.Routing.Application.Queries.GetRouteById;
using FlightDex.Routing.Domain;
using FlightDex.Routing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Routing.Infrastructure;

public static class RoutingModule
{
    public static IServiceCollection AddRoutingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<RoutingDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("FlightDex")));

        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<GetRouteByIdHandler>();

        return services;
    }
}
