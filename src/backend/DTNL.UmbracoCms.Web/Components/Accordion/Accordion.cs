namespace DTNL.UmbracoCms.Web.Components;

public partial class Accordion
{
    public List<Item>? Items { get; set; }

    public bool AutoClose { get; set; }

    public bool CloseOnMobile { get; set; }

    public string? Classes { get; set; }

    public bool TabsOnDesktop { get; set; }

    public bool DisableCloseDesktop { get; set; }
}
