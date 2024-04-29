namespace Hospital.Models;

/// <summary>
/// Represents a visit to a doctor by a patient.
/// </summary>
public class Visit : ICSVParser<Visit>
{
    /// <summary>
    /// The ID of the doctor who performed the visit.
    /// </summary>
    public int DoctorId { get; set; }

    /// <summary>
    /// The ID of the patient who visited the doctor.
    /// </summary>
    public int PatientId { get; set; }

    /// <summary>
    /// The date of the visit.
    /// </summary>
    public DateOnly? Date { get; set; }

    /// <summary>
    /// Parses a string representation of a visit into a Visit object.
    /// </summary>
    /// <param name="s">The string representation of the visit.</param>
    /// <param name="separator">The separator character used in the string representation.</param>
    /// <returns>The Visit object parsed from the string.</returns>
    /// <exception cref="FormatException">Thrown when the string cannot be parsed to a Visit object.</exception>
    public static Visit Parse(string? s, string separator = ";")
    {
        var words = s?.Split(new[] { separator }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (words is null || words.Length is < 2 or > 3 ||
            !int.TryParse(words[0], out var id1) ||
            !int.TryParse(words[1], out var id2))
        {
            throw new FormatException("String cannot be parsed to the instance of Visit type");
        }

        DateOnly date = DateOnly.MinValue;
        if (words.Length == 3 && DateOnly.TryParse(words[2], out date))
        {
            Date = date;
        }
        else
        {
            Date = null;
        }

        return new Visit { DoctorId = id1, PatientId = id2 };
    }

    //public static bool TryParse(string? s, IFormatProvider? provider, out Visit? result) =>
    //    (result = Parse(s)) is not null;
}
