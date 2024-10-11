using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public class GeneralFilters : BaseFilters
{
    public GeneralFilters(PageSearch searchPage, IQueryCollection queryCollection)
        : base(searchPage, queryCollection)
    {
    }
}
