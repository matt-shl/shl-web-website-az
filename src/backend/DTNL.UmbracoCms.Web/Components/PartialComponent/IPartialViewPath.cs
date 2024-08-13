namespace DTNL.UmbracoCms.Web.Components.PartialComponent;

public interface IPartialViewPath
{
    string ViewPath => $"~/Components/{GetType().Name}/{GetType().Name}.cshtml";
}
