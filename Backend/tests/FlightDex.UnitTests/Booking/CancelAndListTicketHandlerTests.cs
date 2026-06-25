using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Commands.CancelTicket;
using FlightDex.Booking.Application.Queries.GetMyTickets;
using FlightDex.Booking.Domain;
using NSubstitute;

namespace FlightDex.UnitTests.Booking;

public sealed class CancelAndListTicketHandlerTests
{
    private readonly ITicketRepository _tickets = Substitute.For<ITicketRepository>();

    private static Ticket TicketOwnedBy(int userId)
    {
        var t = new Ticket(userId, new DateOnly(2026, 7, 1), new TimeOnly(8, 30),
            "BLR", "Kempegowda", "Bengaluru", "DEL", "Indira Gandhi", "Delhi", "Jane", "Doe", 30);
        return t;
    }

    [Fact]
    public async Task Cancel_removes_a_ticket_the_user_owns_and_returns_true()
    {
        _tickets.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(TicketOwnedBy(7));

        var ok = await new CancelTicketCommandHandler(_tickets).HandleAsync(new CancelTicketCommand(7, 5));

        Assert.True(ok);
        await _tickets.Received(1).RemoveAsync(Arg.Any<Ticket>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_refuses_to_touch_a_ticket_owned_by_another_user()
    {
        _tickets.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(TicketOwnedBy(999));

        var ok = await new CancelTicketCommandHandler(_tickets).HandleAsync(new CancelTicketCommand(7, 5));

        Assert.False(ok);   // not the owner → nothing cancelled
        await _tickets.DidNotReceive().RemoveAsync(Arg.Any<Ticket>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_returns_false_when_the_ticket_does_not_exist()
    {
        _tickets.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns((Ticket?)null);

        var ok = await new CancelTicketCommandHandler(_tickets).HandleAsync(new CancelTicketCommand(7, 5));

        Assert.False(ok);
    }

    [Fact]
    public async Task GetMyTickets_maps_owned_tickets_to_dtos()
    {
        _tickets.GetByUserAsync(7, Arg.Any<CancellationToken>())
            .Returns([TicketOwnedBy(7), TicketOwnedBy(7)]);

        var result = await new GetMyTicketsQueryHandler(_tickets).HandleAsync(new GetMyTicketsQuery(7));

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal("Jane", t.FirstName));
    }
}
