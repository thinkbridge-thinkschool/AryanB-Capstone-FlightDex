using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Queries.GetMyTickets;

/// <summary>All tickets belonging to the authenticated user, most recent first.</summary>
public sealed record GetMyTicketsQuery(int UserId) : IQuery<IReadOnlyList<TicketDto>>;
