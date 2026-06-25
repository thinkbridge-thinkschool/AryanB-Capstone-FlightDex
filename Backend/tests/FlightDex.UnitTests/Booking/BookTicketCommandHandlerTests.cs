using FlightDex.Booking.Application.Abstractions;
using FlightDex.Booking.Application.Commands.BookTicket;
using FlightDex.Booking.Domain;
using NSubstitute;

namespace FlightDex.UnitTests.Booking;

public sealed class BookTicketCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly ITicketRepository _tickets = Substitute.For<ITicketRepository>();

    private BookTicketCommandHandler Sut() => new(_users, _tickets);

    private static BookTicketCommand Command(int userId = 7) => new(
        userId,
        new DateOnly(2026, 7, 1),
        new TimeOnly(8, 30),
        "BLR", "Kempegowda International Airport", "Bengaluru",
        "DEL", "Indira Gandhi International Airport", "Delhi");

    [Fact]
    public async Task Throws_when_the_booking_user_does_not_exist()
    {
        _users.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns((User?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => Sut().HandleAsync(Command()));
        await _tickets.DidNotReceive().AddAsync(Arg.Any<Ticket>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Snapshots_the_users_identity_onto_the_persisted_ticket()
    {
        _users.GetByIdAsync(7, Arg.Any<CancellationToken>())
            .Returns(new User("jane@example.com", "Jane", "Doe", 30, false, false, "h", "s"));

        Ticket? saved = null;
        await _tickets.AddAsync(Arg.Do<Ticket>(t => saved = t), Arg.Any<CancellationToken>());

        var dto = await Sut().HandleAsync(Command());

        Assert.NotNull(saved);
        Assert.Equal("Jane", saved!.FirstName);     // taken from the user, not the client
        Assert.Equal("Doe", saved.LastName);
        Assert.Equal(30, saved.Age);
        Assert.Equal("BLR", saved.OriginCode);
        Assert.Equal("DEL", saved.DestinationCode);

        Assert.Equal("2026-07-01", dto.Date);
        Assert.Equal("08:30", dto.Time);
        Assert.Equal("Jane", dto.FirstName);
    }
}
