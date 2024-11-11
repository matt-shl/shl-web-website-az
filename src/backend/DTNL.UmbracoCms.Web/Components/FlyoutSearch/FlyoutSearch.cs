namespace DTNL.UmbracoCms.Web.Components;

public class FlyoutSearch
{
    public required string Id { get; set; }

    public static FlyoutSearch Create(string id)
    {
        return new FlyoutSearch
        {
            Id = id,
        };
    }
}
