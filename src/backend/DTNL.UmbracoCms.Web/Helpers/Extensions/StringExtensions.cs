using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static partial class StringExtensions
{
    /// <inheritdoc cref="string.IsNullOrEmpty"/>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Turns a null or empty string into a null string.
    /// </summary>
    public static string? NullOrEmptyAsNull(this string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>
    /// Returns the <paramref name="firstString"/> if not null, else returns the <paramref name="fallBack"/>.
    /// </summary>
    [return: NotNullIfNotNull(nameof(fallBack))]
    public static string? FallBack(this string? firstString, string? fallBack)
    {
        return string.IsNullOrEmpty(firstString) ? fallBack : firstString;
    }

    /// <summary>
    /// Removes the html tags from a provided <paramref name="text"/>.
    /// </summary>
    public static string RemoveHtml(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        text = text.StripHtml();

        // Remove extra spaces
        text = MultipleConsecutiveSpacesRegex().Replace(text, " ");

        return WebUtility.HtmlDecode(text);
    }

    /// <summary>
    /// Transforms the provided <paramref name="input"/> to conform to a certain <paramref name="length"/>, without cutting words.
    /// </summary>
    public static string TruncateOnWholeWord(this string input, int length, string suffix = "...")
    {
        if (length <= 0 || string.IsNullOrEmpty(input) || input.Length <= length)
        {
            return input;
        }

        string trimmedString = input[..Math.Max(Math.Min(length - suffix.Length, input.Length), 0)];
        return trimmedString.LastIndexOf(' ') is var lastSpaceIndex and >= 0
            ? $"{input[..lastSpaceIndex]}{suffix}"
            : $"{trimmedString}{suffix}";
    }

    [GeneratedRegex(@"\\s{2,}")]
    private static partial Regex MultipleConsecutiveSpacesRegex();
}
