using System.Diagnostics.CodeAnalysis;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Group<T>
    where T : class
{
    public required string Id { get; set; }

    public string? Title { get; set; }

    public required List<T> Items { get; set; }

    [return: NotNullIfNotNull(nameof(linksBlock))]
    public static Group<T>? Create(NestedBlockLinks? linksBlock, Func<Umbraco.Cms.Core.Models.Link, T> toGroupItem)
    {
        if (linksBlock is null)
        {
            return null;
        }

        return new Group<T>
        {
            Id = linksBlock.Key.ToString(),
            Title = linksBlock.Title,
            Items = linksBlock.Links
                .Using(toGroupItem)
                .ToList(),
        };
    }
}
