using Hangfire;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;

public static class JobsScheduler
{
    public static void Schedule<TJob>(string? cronExpression)
        where TJob : IBackgroundJob
    {
        RecurringJob.AddOrUpdate<TJob>(typeof(TJob).Name, t => t.Run(null!), cronExpression ?? Cron.Never());
    }
}
