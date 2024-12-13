using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
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

    public static string ToCamelCase(this string value)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(value);
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

    public static string RemoveSpecialCharacters(this string str)
    {
        StringBuilder sb = new();

        foreach (char c in str)
        {
            if (c is (>= '0' and <= '9') or (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or ' ' or '-')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public static string RemoveDiacritics(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        StringBuilder stringBuilder = new();

        string normalizedString = input.Normalize(NormalizationForm.FormD);

        foreach (char c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString();
    }

    [return: NotNullIfNotNull(nameof(input))]
    public static string? ToUrlString(this string? input)
    {
        return input?
            .RemoveDiacritics()
            .RemoveSpecialCharacters()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(Environment.NewLine, "-")
            .Trim();
    }
}
