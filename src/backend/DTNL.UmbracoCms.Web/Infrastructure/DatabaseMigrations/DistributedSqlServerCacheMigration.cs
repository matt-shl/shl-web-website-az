using System.Data;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

namespace DTNL.UmbracoCms.Web.Infrastructure.DatabaseMigrations;

/// <summary>
/// <see cref="IComposer"/> for <see cref="DistributedSqlServerCacheMigration"/>.
/// </summary>
public class DistributedSqlServerCacheComposer : DatabaseMigrationComposer<DistributedSqlServerCacheMigration>;

/// <summary>
/// Database migration implementation for the Microsoft SQL Server distributed caching services.
/// </summary>
public class DistributedSqlServerCacheMigration : BaseDatabaseMigrationComponent
{
    private readonly IServerRoleAccessor _serverRoleAccessor;
    public const string TableName = "DistributedCache";

    public DistributedSqlServerCacheMigration(
        IScopeProvider scopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState,
        IServerRoleAccessor serverRoleAccessor
    )
        : base(scopeProvider, migrationPlanExecutor, keyValueService, runtimeState)
    {
        _serverRoleAccessor = serverRoleAccessor;
    }

    public override ICollection<RuntimeLevel> SupportedRuntimeLevels { get; } = [RuntimeLevel.Install, RuntimeLevel.Run];

    public override MigrationPlan? BuildMigrationPlan()
    {
        // Not needed in non Load Balanced scenarios
        if (_serverRoleAccessor.CurrentServerRole == ServerRole.Single)
        {
            return null;
        }

        MigrationPlan migrationPlan = new(nameof(DistributedSqlServerCacheMigration));

        // This is the steps we need to take
        // Each step in the migration adds a unique value
        migrationPlan.From(string.Empty)
            .To<InitialTableSchema>(nameof(InitialTableSchema));

        /*
         * New extra steps should be added here
         */

        return migrationPlan;
    }

    public class InitialTableSchema : MigrationBase
    {
        public InitialTableSchema(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            if (TableExists(TableName))
            {
                return;
            }

            // Table schema obtained from:
            // https://github.com/aspnet/DotNetTools/blob/master/src/dotnet-sql-cache/SqlQueries.cs
            Create.Table(TableName)
                .WithColumn("Id").AsString(449)
                .WithColumn("Value").AsBinary()
                .WithColumn("ExpiresAtTime").AsCustom(nameof(DbType.DateTimeOffset))
                .WithColumn("SlidingExpirationInSeconds").AsInt64().Nullable()
                .WithColumn("AbsoluteExpiration").AsCustom(nameof(DbType.DateTimeOffset)).Nullable()
                .Do();

            Create.PrimaryKey($"PK__{TableName}", true).OnTable(TableName).Column("Id").Do();
            Create.Index("Index_ExpiresAtTime").OnTable(TableName).OnColumn("ExpiresAtTime").Ascending().WithOptions().NonClustered().Do();
        }
    }
}
