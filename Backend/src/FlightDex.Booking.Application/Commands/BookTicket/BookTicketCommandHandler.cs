using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.BookTicket;

internal sealed class BookTicketCommandHandler(
    IUserRepository users,
    ITicketRepository tickets) : ICommandHandler<BookTicketCommand, TicketDto>
{
    public Task<TicketDto> HandleAsync(BookTicketCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: load user, snapshot passenger + leg onto a new Ticket, persist, return TicketDto.
        throw new NotImplementedException();
    }
}
