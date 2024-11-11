using System.Linq.Expressions;
using Newtonsoft.Json;
using OurCommunityMediaColourFinder.Interfaces;
using OurCommunityMediaColourFinder.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;
using Image = Umbraco.Cms.Web.Common.PublishedModels.Image;

namespace DTNL.UmbracoCms.Web.Infrastructure.NotificationHandlers;

public class DominantColorNotificationHandler : INotificationAsyncHandler<MediaSavingNotification>
{
    private const string DefaultDominantHexColor = "rgba(0, 0, 0, 0)";
    private readonly IMediaService _mediaService;
    private readonly IColourService _colorService;
    private readonly ILogger<DominantColorNotificationHandler> _logger;
    private readonly string? _dominantColorPropertyName;
    private readonly string? _umbracoFilePropertyName;

    public DominantColorNotificationHandler(
        IMediaService mediaService,
        IColourService colorService,
        IPublishedSnapshotAccessor publishedSnapshotAccessor,
        ILogger<DominantColorNotificationHandler> logger)
    {
        _mediaService = mediaService;
        _colorService = colorService;
        _logger = logger;
        _dominantColorPropertyName = GetImagePropertyName(publishedSnapshotAccessor, image => image.DominantColor);
        _umbracoFilePropertyName = GetImagePropertyName(publishedSnapshotAccessor, image => image.UmbracoFile);
    }

    /// <summary>
    /// Method responsible for saving the dominant color of an uploaded image.
    /// </summary>
    public async Task HandleAsync(MediaSavingNotification notification, CancellationToken cancellationToken)
    {
        if (_dominantColorPropertyName is null or "" || _umbracoFilePropertyName is null or "")
        {
            return;
        }

        foreach (IMedia? mediaItem in notification.SavedEntities)
        {
            switch (mediaItem.ContentType.Alias)
            {
                case UmbracoMediaVectorGraphics.ModelTypeAlias:
                    HandleSvg(notification, mediaItem);
                    break;
                case Image.ModelTypeAlias:
                    await HandleImage(notification, mediaItem);
                    break;
                default:
                    break;
            }
        }
    }

    private void HandleSvg(MediaSavingNotification notification, IContentBase svg)
    {
        if (_dominantColorPropertyName is null or "" || GetCurrentDominantColor(svg, _dominantColorPropertyName) is not (null or ""))
        {
            return;
        }

        svg.SetValue(_dominantColorPropertyName, DefaultDominantHexColor);
        notification.Messages.Add(new EventMessage("Dominant color", "SVG format is not supported. Using default color.", EventMessageType.Info));
    }

    private async Task HandleImage(MediaSavingNotification notification, IContentBase image)
    {
        if (_umbracoFilePropertyName is null or "" || _dominantColorPropertyName is null or "" || GetCurrentDominantColor(image, _dominantColorPropertyName) is not (null or ""))
        {
            return;
        }

        string? imgSrc = GetImageSrc(image, _umbracoFilePropertyName);
        if (imgSrc is null or "")
        {
            return;
        }

        string dominantColorHex = DefaultDominantHexColor;
        try
        {
            await using Stream? imageStream = _mediaService.GetMediaFileContentStream(imgSrc);

            if (imageStream is not null)
            {
                ImageWithColour? colorInfo = _colorService.GetImageWithColour(new FocalPointRectangle
                {
                    Stream = imageStream,
                    Height = image.GetValue<int>(Umbraco.Cms.Core.Constants.Conventions.Media.Height),
                    Width = image.GetValue<int>(Umbraco.Cms.Core.Constants.Conventions.Media.Width),
                    Left = 0.5m,
                    Top = 0.5m,
                });

                if (colorInfo?.Average is not null)
                {
                    dominantColorHex = colorInfo.Average;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get dominant color from {Source}", imgSrc);
            notification.Messages.Add(new EventMessage("Dominant color", "Unable to set the dominant color for this image. Please insert it manually.", EventMessageType.Warning));
        }

        image.SetValue(_dominantColorPropertyName, dominantColorHex);
    }

    private string? GetImageSrc(IContentBase media, string umbracoFilePropertyName)
    {
        if (!media.Properties.TryGetValue(umbracoFilePropertyName, out IProperty? property)
            || property.Values.FirstOrDefault()?.EditedValue is not string jsonString)
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<ImageCropperValue>(jsonString)?.Src;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to deserialize {Property} from media {Id} - {Name}", _umbracoFilePropertyName, media.Id, media.Name);
        }

        return null;
    }

    private static string? GetCurrentDominantColor(IContentBase mediaItem, string dominantColorPropertyName)
    {
        return mediaItem.Properties.TryGetValue(dominantColorPropertyName, out IProperty? property)
            ? property.GetValue() as string
            : null;
    }

    private static string? GetImagePropertyName(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<Image, object?>> selector)
    {
        return Image.GetModelPropertyType(publishedSnapshotAccessor, selector)?.Alias;
    }
}
