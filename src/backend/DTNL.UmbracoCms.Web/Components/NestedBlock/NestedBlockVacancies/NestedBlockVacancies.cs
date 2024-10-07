using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockVacancies : NestedBlockWithInner
{
    protected override JobVacancies? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockVacancies vacanciesBlock ||
            NodeProvider.VacancyOverviewPage is null)
        {
            return null;
        }

        LayoutSection.CssClasses = "c-section-job-vacancies";

        return JobVacancies.Create(vacanciesBlock, NodeProvider.VacancyOverviewPage);
    }
}
