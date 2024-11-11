using Umbraco.Cms.Core.Sections;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Section;

public class BackgroundJobsSection : ISection
{
    public string Alias => BackgroundJobsConstants.SectionAlias;

    public string Name => "Background Jobs";
}
