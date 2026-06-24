using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.UpdateTicket;

/// <summary>
/// Reschedules one of the user's tickets to a new date and time. Returns the updated
/// ticket, or null if it doesn't exist or belongs to a different user — so a user can
/// never edit someone else's ticket.
/// </summary>
public sealed record UpdateTicketCommand(
    int UserId,
    int TicketId,
    DateOnly Date,
    TimeOnly Time) : ICommand<TicketDto?>;
