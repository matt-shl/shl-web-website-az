namespace DTNL.UmbracoCms.Web.Components;

public abstract class PardotForm
{
    public required string ActionUrl { get; set; }

    public abstract string ActionSubmitLabelKey { get; }

    public abstract string ActionSuccessLabelKey { get; }

    public abstract string ActionErrorLabelKey { get; }
}
