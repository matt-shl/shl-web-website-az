using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace DTNL.UmbracoCms.Web.Infrastructure.DatabaseMigrations;

/// <summary>
/// Base <see cref="IComposer"/> implementation for database migrations.
/// </summary>
public abstract class DatabaseMigrationComposer<T> : ComponentComposer<T>
    where T : BaseDatabaseMigrationComponent
{
    public override void Compose(IUmbracoBuilder builder)
    {
        base.Compose(builder);
        builder.AddNotificationHandler<UnattendedInstallNotification, T>();
    }
}

/// <summary>
/// Base <see cref="IComponent"/> implementation for database migrations.
/// </summary>
public abstract class BaseDatabaseMigrationComponent : IComponent, INotificationHandler<UnattendedInstallNotification>
{
    private readonly IScopeProvider _scopeProvider;
    private readonly IMigrationPlanExecutor _migrationPlanExecutor;
    private readonly IKeyValueService _keyValueService;
    private readonly IRuntimeState _runtimeState;

    protected BaseDatabaseMigrationComponent(
        IScopeProvider scopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState)
    {
        _scopeProvider = scopeProvider;
        _migrationPlanExecutor = migrationPlanExecutor;
        _keyValueService = keyValueService;
        _runtimeState = runtimeState;
    }

    /// <summary>
    /// Gets the <see cref="RuntimeLevel"/> under which the migrations should attempt to run.
    /// </summary>
    public abstract ICollection<RuntimeLevel> SupportedRuntimeLevels { get; }

    /// <summary>
    /// Creates a <see cref="MigrationPlan"/> for a specific project/feature.
    /// </summary>
    public abstract MigrationPlan? BuildMigrationPlan();

    /// <summary>
    /// Runs the <see cref="MigrationPlan"/>.
    /// </summary>
    public virtual void Initialize()
    {
        if (!SupportedRuntimeLevels.Contains(_runtimeState.Level))
        {
            return;
        }

        if (BuildMigrationPlan() is not { } migrationPlan)
        {
            return;
        }

        if (_runtimeState.Level == RuntimeLevel.Install)
        {
            // For tables that need to be ready before installation,
            // we execute the plan directly as the migration state tracking table isn't ready yet.
            _ = _migrationPlanExecutor.ExecutePlan(migrationPlan, migrationPlan.InitialState);
        }
        else
        {
            // Go and upgrade our site
            // (It automatically checks if it needs to do the work or not based on the current/latest step)
            new Upgrader(migrationPlan).Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }
    }

    // For unattended installs, ensure we run Install migrations before users are created
    public void Handle(UnattendedInstallNotification notification)
    {
        if (!SupportedRuntimeLevels.Contains(RuntimeLevel.Install))
        {
            return;
        }

        if (BuildMigrationPlan() is not { } migrationPlan)
        {
            return;
        }

        new Upgrader(migrationPlan).Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
    }

    public virtual void Terminate()
    {
    }
}
