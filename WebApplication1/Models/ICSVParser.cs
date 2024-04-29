namespace Hospital.Models;

public interface ICSVParser<T>
{
    T Parse(string s, string separator);
}
