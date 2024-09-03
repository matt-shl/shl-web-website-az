using System.Globalization;

namespace DTNL.UmbracoCms.Web.Components;

public class Odometer
{
    public string? Id { get; set; }

    public string? Direction { get; set; } = "down";

    public required List<char> Digits { get; set; }

    public static Odometer Create(int number)
    {
        return new Odometer
        {
            Digits = number
                .ToString(CultureInfo.InvariantCulture)
                .Select(digit => digit)
                .ToList(),
        };
    }
}
