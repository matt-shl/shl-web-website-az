namespace DTNL.UmbracoCms.Web.Components;

public interface ICard
{
    string ViewPath => $"~/Components/{GetType().Name}/{GetType().Name}.cshtml";
}
