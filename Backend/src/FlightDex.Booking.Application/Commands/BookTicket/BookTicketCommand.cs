using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;

namespace FlightDex.Booking.Application.Commands.BookTicket;

/// <summary>Books a ticket for the signed-in user against a chosen leg.</summary>
public sealed record BookTicketCommand(
    int UserId,
    DateOnly Date,
    TimeOnly Time,
    string OriginCode,
    string OriginAirport,
    string OriginCity,
    string DestinationCode,
    string DestinationAirport,
    string DestinationCity) : ICommand<TicketDto>;
