using System.Net.Http.Json;
using System.Text.Json;

namespace FlightDex.IntegrationTests.Infrastructure;

// Test-side mirrors of the JSON the API returns. Direction is read as a string because
// the API serialises enums as strings.

public sealed record PagedResponse<T>(
    List<T> Items, int Page, int PageSize, int TotalCount, int TotalPages, bool HasPrevious, bool HasNext);

public sealed record FlightItem(
    string FlightCode, string Time, string Airline, string City, string Code, string Airport, string Direction);

public sealed record FlightDetailResponse(
    string FlightCode, string Airport, string Direction, string Time,
    string AirlineCode, string Airline, string CounterpartAirport,
    string CounterpartCode, string CounterpartCity, string Duration);

public sealed record SuggestionResponse(string Code, string Name, string City);

public sealed record UserResponse(
    int Id, string Email, string FirstName, string LastName, int Age,
    bool IsGovernmentOfficial, bool IsLawEnforcementOrMilitary);

public sealed record AuthResponse(string Token, DateTime ExpiresAtUtc, UserResponse User);

public sealed record TicketResponse(
    int TicketId, string Date, string Time,
    string OriginCode, string OriginAirport, string OriginCity,
    string DestinationCode, string DestinationAirport, string DestinationCity,
    string FirstName, string LastName, int Age);

internal static class JsonHttp
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static async Task<T> ReadAsync<T>(this HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(Options);
        return value ?? throw new InvalidOperationException($"Response body was not valid {typeof(T).Name}.");
    }
}
