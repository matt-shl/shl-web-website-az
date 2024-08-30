namespace DTNL.UmbracoCms.Web.Models.Globalization;

public class AlternateUrl
{
    public required string LanguageName { get; set; }

    public required string LanguageCode { get; set; }

    public required string Url { get; set; }

    public required bool IsDefault { get; set; }
}
