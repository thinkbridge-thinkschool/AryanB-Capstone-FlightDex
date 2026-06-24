using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.UpdateTicket;

internal sealed class UpdateTicketCommandHandler(ITicketRepository tickets)
    : ICommandHandler<UpdateTicketCommand, TicketDto?>
{
    public async Task<TicketDto?> HandleAsync(UpdateTicketCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await tickets.GetByIdAsync(command.TicketId, cancellationToken);

        // Not found, or owned by another user → treat as "nothing to update".
        if (ticket is null || ticket.UserId != command.UserId)
            return null;

        ticket.Reschedule(command.Date, command.Time);
        await tickets.UpdateAsync(ticket, cancellationToken);

        return TicketDto.FromDomain(ticket);
    }
}
