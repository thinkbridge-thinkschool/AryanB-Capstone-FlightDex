using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Infrastructure.Persistence;
using FlightDex.Booking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Booking.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// The Booking module's connection string. It points at its own SQLite file, separate
    /// from the Flights timetable DB — two EnsureCreated contexts can't share one file.
    /// </summary>
    public const string ConnectionStringName = "Booking";

    /// <summary>
    /// Registers the Booking module's persistence (Users + Tickets), the password hasher
    /// and the JWT token service.
    /// </summary>
    public static IServiceCollection AddBookingInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured.");

        // SQLite locally (default); Azure SQL when Database:Provider=SqlServer. A dedicated
        // migrations-history table lets Booking and Flights safely share one Azure SQL database.
        var useSqlServer = string.Equals(
            configuration["Database:Provider"], "SqlServer", StringComparison.OrdinalIgnoreCase);

        services.AddDbContext<BookingDbContext>(options =>
        {
            if (useSqlServer)
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly("FlightDex.Booking.Infrastructure.SqlServer");
                    sql.MigrationsHistoryTable("__BookingMigrationsHistory");
                });
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
