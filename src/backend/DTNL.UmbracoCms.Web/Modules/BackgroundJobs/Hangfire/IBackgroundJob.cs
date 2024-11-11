using Hangfire;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;

/// <summary>
/// Background job interface.
/// </summary>
public interface IBackgroundJob
{
    /// <summary>
    /// Runs the <see cref="IBackgroundJob"/>.
    /// </summary>
    Task Run(IJobCancellationToken cancellationToken);
}
