using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Modules.BrandfolderPicker;

public class AddMediaPathToBrandfolderAssetsFilter : IAsyncActionFilter
{
    private static readonly string[] BrandfolderAssetModelTypeAliases =
    [
        BrandfolderImage.ModelTypeAlias,
        BrandfolderFile.ModelTypeAlias,
    ];

    private readonly IMediaService _mediaService;

    public AddMediaPathToBrandfolderAssetsFilter(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ActionExecutedContext executedContext = await next();

        RouteValueDictionary routeData = context.RouteData.Values;

        if (!routeData.TryGetValue("controller", out object? controller) ||
            !(controller?.ToString()).InvariantEquals("Entity") ||
            !routeData.TryGetValue("action", out object? action))
        {
            return;
        }

        IQueryCollection queryParams = context.HttpContext.Request.Query;
        string type = queryParams["type"].ToString();

        if (type != "Media")
        {
            return;
        }

        if ((action?.ToString()).InvariantEquals("GetById"))
        {
            if (executedContext.Result is ObjectResult { Value: EntityBasic mediaItem })
            {
                SetBrandfolerImageUrl(mediaItem);
            }
        }
        else if ((action?.ToString()).InvariantEquals("GetChildren"))
        {
            if (executedContext.Result is ObjectResult { Value: IEnumerable<EntityBasic> mediaItems } objectResult)
            {
                mediaItems = mediaItems.ToList();

                foreach (EntityBasic mediaItem in mediaItems)
                {
                    SetBrandfolerImageUrl(mediaItem);
                }

                objectResult.Value = mediaItems;
            }
        }
    }

    private void SetBrandfolerImageUrl(EntityBasic mediaItem)
    {
        if (!mediaItem.AdditionalData.TryGetValue("ContentTypeAlias", out object? modelTypeAlias) ||
            !BrandfolderAssetModelTypeAliases.Contains(modelTypeAlias as string))
        {
            return;
        }

        IMedia? media = _mediaService.GetById(mediaItem.Key);

        IProperty? mediaFileUrlProperty = media?.Properties.FirstOrDefault(property =>
            property.Alias == nameof(IBrandfolderAsset.BrandfolderUrl).ToFirstLowerInvariant());

        string? thumbnailUrl = mediaFileUrlProperty?.GetValue()?.ToString()?
            .Replace("[\"", "")
            .Replace("\"]", "");

        if (!string.IsNullOrWhiteSpace(thumbnailUrl))
        {
            mediaItem.AdditionalData.TryAdd("MediaPath", thumbnailUrl);
        }
    }
}
