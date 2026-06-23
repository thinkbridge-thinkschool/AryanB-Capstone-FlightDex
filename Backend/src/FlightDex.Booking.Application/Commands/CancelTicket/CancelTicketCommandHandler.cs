using FlightDex.Booking.Application.Abstractions;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.CancelTicket;

internal sealed class CancelTicketCommandHandler(ITicketRepository tickets)
    : ICommandHandler<CancelTicketCommand, bool>
{
    public Task<bool> HandleAsync(CancelTicketCommand command, CancellationToken cancellationToken = default)
    {
        // TODO: delete the ticket only if it belongs to command.UserId.
        throw new NotImplementedException();
    }
}
