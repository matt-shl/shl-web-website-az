namespace DTNL.UmbracoCms.Web.Components;

public class Quote
{
    public required string Quotetext { get; set; }

    public required string Name { get; set; }

    public string? Role { get; set; }

    public string? Company { get; set; }

    public Image? Image { get; set; }
}
