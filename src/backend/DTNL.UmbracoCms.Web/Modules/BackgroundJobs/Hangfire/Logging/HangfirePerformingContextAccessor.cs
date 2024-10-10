using Hangfire.Server;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire.Logging;

/// <summary>
/// Singleton used to keep track and allow access of Hangfire jobs context.
/// </summary>
internal sealed class HangfirePerformingContextAccessor : IServerFilter
{
    private static readonly AsyncLocal<PerformingContext> LocalStorage = new();

    public static PerformingContext? Value => LocalStorage.Value;

    public void OnPerforming(PerformingContext filterContext)
    {
        LocalStorage.Value = filterContext;
    }

    public void OnPerformed(PerformedContext filterContext)
    {
#pragma warning disable CS8625
        LocalStorage.Value = null;
#pragma warning restore CS8625
    }
}
