using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Hospital.Models
{
    public class User : ICSVParser<User>
    {
        private static readonly Regex _pattern = new(@"\t");

        [RegularExpression(@"^\S{2,40}$")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^\S{5,40}$")]
        public string Password { get; set; } = string.Empty;

        public static User Parse(string s, string separator)
        {
            if (separator != "\t")
            {
                throw new ArgumentException("The separator must be a tab character.", nameof(separator));
            }

            var words = _pattern.Split(s);
            if (words.Length != 2)
            {
                throw new FormatException($"Error in the user parsing from the line {s}");
            }

            if (string.IsNullOrWhiteSpace(words[0]) || words[0].Length < 2)
            {
                throw new FormatException("The name must not be empty and must be at least 2 characters long.");
            }

            if (string.IsNullOrWhiteSpace(words[1]) || words[1].Length < 5)
            {
                throw new FormatException("The password must not be empty and must be at least 5 characters long.");
            }

            return new User { Name = words[0], Password = words[1] };
        }

        public override string ToString() => $"{Name}\t{Password}";
    }
}
