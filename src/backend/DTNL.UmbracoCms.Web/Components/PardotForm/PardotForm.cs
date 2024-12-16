namespace DTNL.UmbracoCms.Web.Components;

public abstract class PardotForm
{
    public required string Id { get; set; }

    public required string ActionUrl { get; set; }

    public abstract string ActionSubmitLabelKey { get; }

    public abstract string ActionSuccessLabelKey { get; }

    public abstract string ActionErrorLabelKey { get; }

    public virtual string? ConsentFieldName => null;

    public virtual string SourcePageTitleFieldName => "source_page_title";

    public virtual string SourceUrlFieldName => "source_URL";

    public Dictionary<string, string?> Attributes { get; set; } = [];
}
