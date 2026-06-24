using FlightDex.Booking.Application.Commands.BookTicket;
using FlightDex.Booking.Application.Commands.CancelTicket;
using FlightDex.Booking.Application.Commands.Login;
using FlightDex.Booking.Application.Commands.RegisterUser;
using FlightDex.Booking.Application.Commands.UpdateTicket;
using FlightDex.Booking.Application.Dtos;
using FlightDex.Booking.Application.Queries.GetMyTickets;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.Booking.Application;

public static class DependencyInjection
{
    /// <summary>Registers the Booking module's CQRS command and query handlers.</summary>
    public static IServiceCollection AddBookingApplication(this IServiceCollection services)
    {
        services.AddScoped<
            ICommandHandler<RegisterUserCommand, AuthResult>,
            RegisterUserCommandHandler>();

        services.AddScoped<
            ICommandHandler<LoginCommand, AuthResult>,
            LoginCommandHandler>();

        services.AddScoped<
            ICommandHandler<BookTicketCommand, TicketDto>,
            BookTicketCommandHandler>();

        services.AddScoped<
            ICommandHandler<CancelTicketCommand, bool>,
            CancelTicketCommandHandler>();

        services.AddScoped<
            ICommandHandler<UpdateTicketCommand, TicketDto?>,
            UpdateTicketCommandHandler>();

        services.AddScoped<
            IQueryHandler<GetMyTicketsQuery, IReadOnlyList<TicketDto>>,
            GetMyTicketsQueryHandler>();

        return services;
    }
}
