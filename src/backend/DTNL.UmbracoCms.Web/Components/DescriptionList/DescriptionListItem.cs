using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Products;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class DescriptionList
{
    public class DescriptionListItem
    {
        public required string Title { get; set; }

        public required string Text { get; set; }

        public static DescriptionListItem Create(NestedBlockProductSpecification productSpecification)
        {
            return new DescriptionListItem
            {
                Title = productSpecification.Title!,
                Text = productSpecification.Text!,
            };
        }

        public static IEnumerable<DescriptionListItem> CreateFor(PageProduct? productPage)
        {
            if (productPage is null)
            {
                yield break;
            }

            foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                     in ProductFilters.FilterFields)
            {
                string specificationText = string.Join(',', getValues(productPage).OrEmptyIfNull());

                if (!specificationText.IsNullOrWhiteSpace())
                {
                    yield return new DescriptionListItem
                    {
                        Title = name,
                        Text = specificationText,
                    };
                }
            }
        }
    }
}
