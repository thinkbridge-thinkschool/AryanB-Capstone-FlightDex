namespace FlightDex.Booking.Domain;

/// <summary>
/// A booked ticket owned by a <see cref="User"/>. The origin/destination airport details
/// and the passenger's name + age are snapshotted onto the ticket at booking time so it
/// stays a faithful record even if the user later edits their profile.
/// </summary>
public sealed class Ticket
{
    public int Id { get; private set; }

    /// <summary>Owner of the ticket (the authenticated user who booked it).</summary>
    public int UserId { get; private set; }

    public DateOnly Date { get; private set; }
    public TimeOnly Time { get; private set; }

    public string OriginCode { get; private set; } = string.Empty;
    public string OriginAirport { get; private set; } = string.Empty;
    public string OriginCity { get; private set; } = string.Empty;

    public string DestinationCode { get; private set; } = string.Empty;
    public string DestinationAirport { get; private set; } = string.Empty;
    public string DestinationCity { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public int Age { get; private set; }

    private Ticket() { } // EF Core

    public Ticket(
        int userId,
        DateOnly date,
        TimeOnly time,
        string originCode,
        string originAirport,
        string originCity,
        string destinationCode,
        string destinationAirport,
        string destinationCity,
        string firstName,
        string lastName,
        int age)
    {
        UserId = userId;
        Date = date;
        Time = time;
        OriginCode = originCode;
        OriginAirport = originAirport;
        OriginCity = originCity;
        DestinationCode = destinationCode;
        DestinationAirport = destinationAirport;
        DestinationCity = destinationCity;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    /// <summary>
    /// Reschedules the ticket to a new travel date and time. The route and the
    /// snapshotted passenger details are left unchanged.
    /// </summary>
    public void Reschedule(DateOnly date, TimeOnly time)
    {
        Date = date;
        Time = time;
    }
}
