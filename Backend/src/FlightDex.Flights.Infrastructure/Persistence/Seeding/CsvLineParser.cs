namespace FlightDex.Flights.Infrastructure.Persistence.Seeding;

/// <summary>
/// Minimal RFC-4180-style parser for one CSV line: comma-separated, fields optionally
/// double-quoted, with "" as an escaped quote inside a quoted field. Sufficient for the
/// timetable exports — no embedded newlines.
/// </summary>
internal static class CsvLineParser
{
    public static IReadOnlyList<string> Parse(string line)
    {
        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"') { field.Append('"'); i++; }
                    else inQuotes = false;
                }
                else field.Append(c);
            }
            else if (c == '"') inQuotes = true;
            else if (c == ',') { fields.Add(field.ToString()); field.Clear(); }
            else field.Append(c);
        }

        fields.Add(field.ToString());
        return fields;
    }
}
