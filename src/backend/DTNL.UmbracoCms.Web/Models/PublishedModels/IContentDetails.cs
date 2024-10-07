namespace Umbraco.Cms.Web.Common.PublishedModels;

public partial interface ICompositionContentDetails
{
    DateTime? GetDate()
    {
        return Date == default ? null : Date;
    }
}
