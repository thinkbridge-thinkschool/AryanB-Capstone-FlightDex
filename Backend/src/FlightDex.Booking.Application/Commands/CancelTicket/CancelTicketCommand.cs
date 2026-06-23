using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.CancelTicket;

/// <summary>Cancels one of the signed-in user's tickets. Returns true if a row was removed.</summary>
public sealed record CancelTicketCommand(int UserId, int TicketId) : ICommand<bool>;
