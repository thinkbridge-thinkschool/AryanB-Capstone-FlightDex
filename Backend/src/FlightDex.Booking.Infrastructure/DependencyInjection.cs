using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Infrastructure.Persistence;
using FlightDex.Booking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Booking.Infrastructure;

public static class DependencyInjection
{
    /// <summary>The Booking module's own SQLite file, separate from the Flights timetable DB.</summary>
    public const string ConnectionStringName = "Booking";

    /// <summary>Registers Booking persistence (Users + Tickets), the password hasher and JWT service.</summary>
    public static IServiceCollection AddBookingInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<BookingDbContext>(options => options.UseSqlite(connectionString));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }
}
