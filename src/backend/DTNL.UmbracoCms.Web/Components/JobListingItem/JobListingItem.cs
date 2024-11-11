using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class JobListingItem : IOverviewItem
{
    public required string Url { get; set; }

    public required string Title { get; set; }

    public string? Country { get; set; }

    public required string[] Tags { get; set; }

    public static JobListingItem Create(PageVacancy vacancyPage)
    {
        return new()
        {
            Url = vacancyPage.Url(),
            Title = vacancyPage.GetTitle(),
            Country = vacancyPage.Country,
            Tags = new[] { vacancyPage.JobLevel, vacancyPage.ContractType }
                .EnsureNotNull()
                .ToArray(),
        };
    }
}
