using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public abstract class NestedBlockWithInner : NestedBlock
{
    protected override string ViewPath => "~/Components/NestedBlock/NestedBlockWithInner.cshtml";

    public virtual LayoutSection LayoutSection => new();

    public object? InnerComponent { get; set; }

    public virtual string InnerPartial => $"~/Components/{InnerViewName}/{InnerViewName}.cshtml";

    public virtual string InnerViewName => InnerComponent!.GetType().Name;

    protected override object? ProcessBlock(IPublishedElement block)
    {
        InnerComponent = GetInnerComponent(block);

        return InnerComponent is null ? null : this;
    }

    protected override async Task<object?> ProcessBlockAsync(IPublishedElement block)
    {
        InnerComponent = await GetInnerComponentAsync(block);

        return InnerComponent is null ? null : this;
    }

    protected virtual object? GetInnerComponent(IPublishedElement block)
    {
        throw new NotImplementedException(
            $"{GetType().FullName} must implement either {nameof(GetInnerComponent)} or {nameof(GetInnerComponentAsync)}"
        );
    }

    protected virtual Task<object?> GetInnerComponentAsync(IPublishedElement block)
    {
        return Task.FromResult(GetInnerComponent(block));
    }
}
