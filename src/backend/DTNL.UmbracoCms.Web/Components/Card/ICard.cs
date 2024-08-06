using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public interface ICard
{
    string ViewPath => $"~/Components/{GetType().Name}/{GetType().Name}.cshtml";

    static ICard? Create(IPublishedElement card, string? cssClasses = null)
    {
        return card switch
        {
            NestedBlockImageCard imageCard => CardImage.Create(imageCard, cssClasses),
            NestedBlockNumberCard numberCard => CardNumber.Create(numberCard, cssClasses),
            NestedBlockIconCard iconCard => CardIcon.Create(iconCard, cssClasses),
            NestedBlockPageCard pageCard => CardKnowledge.Create(pageCard, cssClasses),
            _ => null,
        };
    }
}
