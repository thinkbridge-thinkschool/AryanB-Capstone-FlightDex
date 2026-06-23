using FlightDex.Booking.Application.Abstractions;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.CancelTicket;

internal sealed class CancelTicketCommandHandler(ITicketRepository tickets)
    : ICommandHandler<CancelTicketCommand, bool>
{
    public async Task<bool> HandleAsync(CancelTicketCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await tickets.GetByIdAsync(command.TicketId, cancellationToken);

        // Not found, or owned by another user → treat as "nothing to cancel".
        if (ticket is null || ticket.UserId != command.UserId)
            return false;

        await tickets.RemoveAsync(ticket, cancellationToken);
        return true;
    }
}
