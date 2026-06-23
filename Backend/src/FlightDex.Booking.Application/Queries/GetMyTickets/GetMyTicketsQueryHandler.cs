using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Queries.GetMyTickets;

internal sealed class GetMyTicketsQueryHandler(ITicketRepository tickets)
    : IQueryHandler<GetMyTicketsQuery, IReadOnlyList<TicketDto>>
{
    public Task<IReadOnlyList<TicketDto>> HandleAsync(GetMyTicketsQuery query, CancellationToken cancellationToken = default)
    {
        // TODO: load the user's tickets and project to TicketDto.
        throw new NotImplementedException();
    }
}
