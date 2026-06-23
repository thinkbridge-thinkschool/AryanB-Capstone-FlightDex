using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.Booking.Domain;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.BookTicket;

internal sealed class BookTicketCommandHandler(
    IUserRepository users,
    ITicketRepository tickets) : ICommandHandler<BookTicketCommand, TicketDto>
{
    public async Task<TicketDto> HandleAsync(BookTicketCommand command, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new InvalidOperationException($"User {command.UserId} not found.");

        var ticket = new Ticket(
            user.Id,
            command.Date,
            command.Time,
            command.OriginCode,
            command.OriginAirport,
            command.OriginCity,
            command.DestinationCode,
            command.DestinationAirport,
            command.DestinationCity,
            // Passenger identity is the user's own — snapshotted at booking time.
            user.FirstName,
            user.LastName,
            user.Age);

        await tickets.AddAsync(ticket, cancellationToken);
        return TicketDto.FromDomain(ticket);
    }
}
