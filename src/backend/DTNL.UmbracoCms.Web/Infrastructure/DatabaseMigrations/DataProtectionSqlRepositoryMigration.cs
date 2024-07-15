using DTNL.UmbracoCms.Web.Infrastructure.DataProtection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

namespace DTNL.UmbracoCms.Web.Infrastructure.DatabaseMigrations;

/// <summary>
/// <see cref="IComposer"/> for <see cref="DataProtectionSqlRepositoryMigration"/>.
/// </summary>
public class DataProtectionSqlRepositoryComposer : DatabaseMigrationComposer<DataProtectionSqlRepositoryMigration>;

/// <summary>
/// Database migration implementation for <see cref="DataProtectionSqlRepository"/>>.
/// </summary>
public class DataProtectionSqlRepositoryMigration : BaseDatabaseMigrationComponent
{
    public const string TableName = nameof(DataProtectionKey);
    private readonly IServerRoleAccessor _serverRoleAccessor;

    public DataProtectionSqlRepositoryMigration(
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

        MigrationPlan migrationPlan = new(nameof(DataProtectionSqlRepositoryMigration));

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
            // https://github.com/dotnet/aspnetcore/blob/main/src/DataProtection/EntityFrameworkCore/src/IDataProtectionKeyContext.cs
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().PrimaryKey($"PK__{TableName}").Identity()
                .WithColumn("FriendlyName").AsString().Nullable()
                .WithColumn("Xml").AsCustom("nvarchar(MAX)").Nullable()
                .Do();
        }
    }
}
