using System.Net;
using FlightDex.IntegrationTests.Infrastructure;

namespace FlightDex.IntegrationTests;

/// <summary>
/// API-level integration: a real HTTP request travels the whole pipeline — routing →
/// controller → CQRS dispatcher → handler → EF Core repository → SQLite — against the
/// known timetable seeded by the factory.
/// </summary>
[Collection("api")]
public sealed class FlightApiTests(FlightDexApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Default_query_returns_an_airports_departures_sorted_by_time()
    {
        var response = await _client.GetAsync("/flight?at=BLR");
        response.EnsureSuccessStatusCode();

        var page = await response.ReadAsync<PagedResponse<FlightItem>>();

        Assert.Equal(5, page.TotalCount);                       // 5 BLR departures
        Assert.All(page.Items, i => Assert.Equal("Departure", i.Direction));
        Assert.Equal("AI501", page.Items[0].FlightCode);        // 06:00 is earliest
        Assert.Equal(
            page.Items.Select(i => i.Time),
            page.Items.Select(i => i.Time).OrderBy(t => t));     // ascending by time
    }

    [Fact]
    public async Task To_filter_restricts_departures_to_a_destination()
    {
        var page = await (await _client.GetAsync("/flight?at=BLR&to=DEL")).ReadAsync<PagedResponse<FlightItem>>();

        Assert.Equal(4, page.TotalCount);
        Assert.All(page.Items, i => Assert.Equal("DEL", i.Code));
    }

    [Fact]
    public async Task Presence_of_from_selects_arrivals()
    {
        var page = await (await _client.GetAsync("/flight?at=BLR&from")).ReadAsync<PagedResponse<FlightItem>>();

        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Items, i => Assert.Equal("Arrival", i.Direction));
    }

    [Fact]
    public async Task Departure_time_window_filters_inclusively()
    {
        var page = await (await _client.GetAsync("/flight?at=BLR&deptTime_After=0800&deptTime_Before=1500"))
            .ReadAsync<PagedResponse<FlightItem>>();

        Assert.Equal(["AI502", "6E101", "UK810"], page.Items.Select(i => i.FlightCode));   // 08:05, 09:30, 14:00
    }

    [Fact]
    public async Task Paging_caps_the_page_and_reports_navigation_metadata()
    {
        var page = await (await _client.GetAsync("/flight?at=BLR&page=1&pageSize=2")).ReadAsync<PagedResponse<FlightItem>>();

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(5, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.False(page.HasPrevious);
        Assert.True(page.HasNext);
    }

    [Fact]
    public async Task Unserved_airport_is_rejected_with_400()
    {
        var response = await _client.GetAsync("/flight?at=XYZ");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Malformed_time_filter_is_rejected_with_400()
    {
        var response = await _client.GetAsync("/flight?at=BLR&deptTime_After=99");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Detail_by_code_returns_a_single_unwrapped_flight()
    {
        var detail = await (await _client.GetAsync("/flight/AI501")).ReadAsync<FlightDetailResponse>();

        Assert.Equal("AI501", detail.FlightCode);
        Assert.Equal("BLR", detail.Airport);
        Assert.Equal("DEL", detail.CounterpartCode);
    }

    [Fact]
    public async Task Unknown_flight_code_returns_404()
    {
        var response = await _client.GetAsync("/flight/ZZ999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Suggestions_endpoint_serves_the_locations_table()
    {
        var suggestions = await (await _client.GetAsync("/airports/suggestions")).ReadAsync<List<SuggestionResponse>>();

        Assert.Equal(3, suggestions.Count);
        Assert.Contains(suggestions, s => s.Code == "DEL" && s.City == "Delhi");
    }

    [Fact]
    public async Task Booking_endpoints_require_authentication()
    {
        var response = await _client.GetAsync("/ticket");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
