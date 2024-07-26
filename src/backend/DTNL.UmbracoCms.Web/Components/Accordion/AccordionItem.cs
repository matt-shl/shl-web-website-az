namespace DTNL.UmbracoCms.Web.Components;

public partial class Accordion
{
    public class Item
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public bool Open { get; set; }

        public string? Classes { get; set; }
    }
}
