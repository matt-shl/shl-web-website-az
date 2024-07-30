using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace DTNL.UmbracoCms.Web.Infrastructure.NotificationHandlers;

/// <summary>
/// Notification handler responsible for flushing the cache.
/// </summary>
public class CacheFlushingNotificationHandler :
    INotificationHandler<ContentCacheRefresherNotification>,
    INotificationHandler<MediaCacheRefresherNotification>
{
    private readonly ICacheManager _cacheManager;

    public CacheFlushingNotificationHandler(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    /// <summary>
    /// Handles <see cref="ContentCacheRefresherNotification"/>.
    /// </summary>
    public void Handle(ContentCacheRefresherNotification notification)
    {
        _cacheManager.Flush();
    }

    /// <summary>
    /// Handles <see cref="MediaCacheRefresherNotification"/>.
    /// </summary>
    public void Handle(MediaCacheRefresherNotification notification)
    {
        _cacheManager.Flush();
    }
}
