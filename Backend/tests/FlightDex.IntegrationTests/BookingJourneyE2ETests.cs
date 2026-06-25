using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FlightDex.IntegrationTests.Infrastructure;

namespace FlightDex.IntegrationTests;

/// <summary>
/// The one full end-to-end journey, driven entirely over HTTP against the real app:
/// register → search the timetable → book a ticket (authenticated) → see it listed →
/// cancel it → confirm it is gone. Exercises auth, the JWT pipeline, both modules and
/// the database together.
/// </summary>
[Collection("api")]
public sealed class BookingJourneyE2ETests(FlightDexApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_search_book_list_then_cancel()
    {
        // 1. Register — a brand-new account, signed in immediately with a bearer token.
        var email = $"traveller-{Guid.NewGuid():N}@example.com";
        var register = await _client.PostAsJsonAsync("/auth/register", new
        {
            email,
            firstName = "Ada",
            lastName = "Lovelace",
            age = 28,
            isGovernmentOfficial = false,
            isLawEnforcementOrMilitary = false,
            password = "Sup3rSecret!",
        });
        register.EnsureSuccessStatusCode();
        var auth = await register.ReadAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
        Assert.Equal(email, auth.User.Email);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        // 2. Search the timetable for a flight to book.
        var flights = await (await _client.GetAsync("/flight?at=BLR&to=DEL")).ReadAsync<PagedResponse<FlightItem>>();
        Assert.NotEmpty(flights.Items);
        var leg = flights.Items[0];

        // 3. Book it. Passenger identity comes from the token, not the request body.
        var bookResponse = await _client.PostAsJsonAsync("/ticket", new
        {
            date = "2026-07-01",
            time = leg.Time,
            origin = new { code = "BLR", airport = "Kempegowda International Airport", city = "Bengaluru" },
            destination = new { code = leg.Code, airport = leg.Airport, city = leg.City },
        });
        Assert.Equal(HttpStatusCode.Created, bookResponse.StatusCode);
        var booked = await bookResponse.ReadAsync<TicketResponse>();
        Assert.Equal("Ada", booked.FirstName);          // snapshotted from the user
        Assert.Equal("DEL", booked.DestinationCode);

        // 4. It shows up in the user's tickets.
        var mine = await (await _client.GetAsync("/ticket")).ReadAsync<List<TicketResponse>>();
        Assert.Contains(mine, t => t.TicketId == booked.TicketId);

        // 5. Cancel it.
        var cancel = await _client.DeleteAsync($"/ticket/{booked.TicketId}");
        Assert.Equal(HttpStatusCode.NoContent, cancel.StatusCode);

        // 6. It is gone.
        var after = await (await _client.GetAsync("/ticket")).ReadAsync<List<TicketResponse>>();
        Assert.DoesNotContain(after, t => t.TicketId == booked.TicketId);
    }
}
