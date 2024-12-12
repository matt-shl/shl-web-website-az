using System.Reflection.Emit;
using Umbraco.Cms.Core.Dictionary;

namespace DTNL.UmbracoCms.Web.Components;

public class SearchTrigger
{
    public required string SearchLabel { get; set; }

    public static SearchTrigger Create(string label)
    {
        return new SearchTrigger
        {
            SearchLabel = label
        };
    }
}
