using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;

namespace DTNL.UmbracoCms.SourceGenerators.Helpers;

internal static partial class StringExtensions
{
    private static readonly Regex SpecialCharsRegex = new("[^a-zA-Z0-9]+", RegexOptions.Compiled);

    public static string ToSafeIdentifier(this string value)
    {
        // Clean special chars
        value = SpecialCharsRegex.Replace(value, " ").Trim();

        // Convert to Title Case
        value = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value);

        // Remove spaces
        value = value.Replace(" ", "");

        // Identifiers can't start with 0-9
        return char.IsDigit(value.FirstOrDefault()) ? $"_{value}" : value;
    }

    public static string ToStringLiteral(this string input, bool quotes = true)
    {
        return SymbolDisplay.FormatLiteral(input, quotes);
    }

    public static string EnsureEndsWith(this string str, string suffix)
    {
        return str.EndsWith(suffix) ? str : str + suffix;
    }
}
