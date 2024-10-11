namespace DTNL.UmbracoCms.Web.Components;

public abstract class PardotForm
{
    public required string ActionUrl { get; set; }

    public required string ActionSubmitLabelKey { get; set; }

    public required string ActionSuccessLabelKey { get; set; }

    public required string ActionErrorLabelKey { get; set; }
}
