namespace DTNL.UmbracoCms.Web.Components;

public class Modal
{
    public string? Size { get; set; }

    public string? Title { get; set; }

    public string? Subtitle { get; set; }

    public string? Class { get; set; }

    public string? Id { get; set; }

    public bool MobileOnly { get; set; }

    public bool AutoFocus { get; set; }

    public bool NoBodyClass { get; set; }

    public bool CloseAllOthers { get; set; }

    public bool KeepScrollPosition { get; set; }

    public bool AutoClose { get; set; }

    public bool NoClose { get; set; }

    public string? Hook { get; set; }
}
