using DEPT.Umbraco.SourceGenerators.TranslationAliases;

namespace DTNL.UmbracoCms.Web.Components.BasePage;

public class SocialChannel
{
    public required string Id { get; set; }

    public required TranslationEntry Label { get; set; }

    public required string Icon { get; set; }

    public string? CssClasses { get; set; }
}
