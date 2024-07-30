using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Sync;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration;

/// <summary>
/// Gets the current server's <see cref="ServerRole"/> based on configuration.
/// </summary>
public class ConfigurationServerRoleAccessor : IServerRoleAccessor
{
    private readonly IOptions<ApplicationOptions> _options;

    public ConfigurationServerRoleAccessor(IOptions<ApplicationOptions> options)
    {
        _options = options;
    }

    public ServerRole CurrentServerRole => _options.Value.ServerRole;
}
