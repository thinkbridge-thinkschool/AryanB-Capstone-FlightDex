using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Queries.GetMyTickets;

/// <summary>The signed-in user's tickets, most recent first.</summary>
public sealed record GetMyTicketsQuery(int UserId) : IQuery<IReadOnlyList<TicketDto>>;
