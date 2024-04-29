using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Hospital.Models;

public class Doctor : ICSVParser<Doctor>
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Speciality { get; set; } = string.Empty;

    public static Doctor Parse(string s, string separator = ";")
    {
        var words = s.Split(new[] { separator }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (words.Length != 3 || !int.TryParse(words[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            throw new FormatException("String cannot be parsed to the instance of Doctor type");
        }

        return new Doctor { Id = id, Name = words[1], Speciality = words[2] };
    }

    public string ToString(string separator = ";")
    {
        return $"{Id}{separator}{Name}{separator}{Speciality}";
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Doctor? result)
    {
        result = null;

        if (s is null)
        {
            return false;
        }

        try
        {
            var words = s.Split(new[] { separator }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (words.Length != 3 || !int.TryParse(words[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                throw new FormatException("String cannot be parsed to the instance of Doctor type");
            }

            result = new Doctor { Id = id, Name = words[1], Speciality = words[2] };
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}
