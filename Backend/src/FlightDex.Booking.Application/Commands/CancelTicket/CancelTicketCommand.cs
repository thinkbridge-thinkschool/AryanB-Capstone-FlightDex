using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.CancelTicket;

/// <summary>
/// Cancels (deletes) one of the user's tickets. Returns false if the ticket doesn't
/// exist or belongs to a different user — so a user can never cancel someone else's.
/// </summary>
public sealed record CancelTicketCommand(int UserId, int TicketId) : ICommand<bool>;
