using DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;
using DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire.Filters;
using DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire.Logging;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Authorization;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;

/// <summary>
/// Hangfire configuration helper methods.
/// </summary>
public static class HangfireConfigurationExtensions
{
    /// <summary>
    /// Registers Hangfire.
    /// </summary>
    public static void AddHangfire(this IServiceCollection services)
    {
        // Add Hangfire services.
        services.AddHangfire((sp, config) =>
        {
            _ = config
                .UseConsole(new ConsoleOptions { BackgroundColor = "#0c0c0c", })
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(sp.GetRequiredService<IOptions<ConnectionStrings>>().Value.ConnectionString!, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                })
                .UseFilter(new HangfirePerformingContextAccessor())
                .UseFilter(new PreserveOriginalQueueAttribute())
                .UseFilter(new AutomaticRetryAttribute { Attempts = 2 })
                .UseFilter(new SkipWhenPreviousJobIsRunningAttribute());
        });

        services.AddHangfireServer();

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(BackgroundJobsConstants.PolicyName, policy =>
            {
                _ = policy
                    .AddAuthenticationSchemes(Umbraco.Cms.Core.Constants.Security.BackOfficeAuthenticationType)
                    .AddRequirements(new SectionRequirement(BackgroundJobsConstants.SectionAlias));
            });
        });
    }
    public static void ConfigureHangfireDashboard(this IUmbracoEndpointBuilderContext configureUmbraco)
    {
        _ = configureUmbraco.EndpointRouteBuilder.MapHangfireDashboard(
                "/umbraco/backoffice/hangfire",
                new DashboardOptions
                {
                    IsReadOnlyFunc = context =>
                    {
                        IBackOfficeSecurityAccessor backOfficeSecurityAccessor = context
                            .GetHttpContext()
                            .RequestServices
                            .GetRequiredService<IBackOfficeSecurityAccessor>();

                        IUser? user = backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;

                        return user?.IsAdmin() != true && user?.Groups.Any(g => g.Alias == "admin") != true;
                    },
                    AppPath = null,
                    Authorization = new List<IDashboardAuthorizationFilter>(),
                })
            .RequireAuthorization(BackgroundJobsConstants.PolicyName);
    }

    /// <summary>
    /// Schedules the background jobs.
    /// </summary>
    public static void ScheduleJob<TJob>(this WebApplication app, string? cronExpression)
        where TJob : IBackgroundJob
    {
        IRuntimeState runtimeState = app.Services.GetRequiredService<IRuntimeState>();

        // Don't register jobs in Dev on first install
        // This is needed because the database isn't ready yet, and an exception would be thrown.
        if (!app.Environment.IsDevelopment() || runtimeState.Level == RuntimeLevel.Run)
        {
            JobsScheduler.Schedule<TJob>(cronExpression);
        }
    }

    /// <summary>
    /// Configures the Hangfire Context enricher for serilog.
    /// </summary>
    /// <remarks>Gets called dynamically by Serilog Configuration based on the appsettings.</remarks>
    public static LoggerConfiguration WithHangfireContext(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<HangfireContextSerilogEnricher>();
    }

    /// <summary>
    /// Configures the Hangfire Console sink for serilog.
    /// </summary>
    /// <remarks>Gets called dynamically by Serilog Configuration based on the appsettings.</remarks>
    public static LoggerConfiguration HangfireConsole(
        this LoggerSinkConfiguration loggerConfiguration,
        IFormatProvider? formatProvider = null,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
        LoggingLevelSwitch? levelSwitch = null)
    {
        return loggerConfiguration.Sink(new HangfireConsoleSerilogSink(formatProvider), restrictedToMinimumLevel, levelSwitch);
    }
}
