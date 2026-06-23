using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Queries.GetMyTickets;

internal sealed class GetMyTicketsQueryHandler(ITicketRepository tickets)
    : IQueryHandler<GetMyTicketsQuery, IReadOnlyList<TicketDto>>
{
    public async Task<IReadOnlyList<TicketDto>> HandleAsync(
        GetMyTicketsQuery query, CancellationToken cancellationToken = default)
    {
        var owned = await tickets.GetByUserAsync(query.UserId, cancellationToken);
        return owned.Select(TicketDto.FromDomain).ToList();
    }
}
