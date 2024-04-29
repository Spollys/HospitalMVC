namespace Hospital.Models;

public class Patient : ICSVParser<Patient>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }

    public static Patient Parse(string? s, string separator = ";")
    {
        if (TryParse(s, separator, out var patient))
            return patient;
        else
            throw new FormatException("String cannot be parsed to the instance of Patient type");
    }

    public static bool TryParse(string? s, string separator = ";", out Patient? patient)
    {
        patient = null;
        if (string.IsNullOrWhiteSpace(s))
            return false;

        var words = s.Split(new[] { separator }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (words.Length != 3 || !int.TryParse(words[0], out var id) ||
            !DateOnly.TryParseExact(words[2], "yyyy-MM-dd", null, out var date))
            return false;

        patient = new Patient { Id = id, Name = words[1], BirthDate = date };
        return true;
    }

    public bool TryParseFromRecord(string[] record)
    {
        if (record.Length != 3 || !int.TryParse(record[0], out var id) ||
            !DateOnly.TryParseExact(record[2], "yyyy-MM-dd", null, out var date))
            return false;

        Id = id;
        Name = record[1];
        BirthDate = date;
        return true;
    }
}
