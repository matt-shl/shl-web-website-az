using System.Diagnostics;
using Azure.Identity;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration;

/// <summary>
///     Extension class for registering AzureKeyVaultExtensions with <see cref="IConfigurationBuilder"/>
///     <see href="https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-5.0"/>
/// </summary>
internal static class AzureKeyVaultConfigurationExtensions
{
    /// <summary>
    /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values from the Azure KeyVault.
    /// </summary>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddAzureKeyVault(
        this ConfigurationManager configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        AzureKeyVaultOptions? options = configuration.GetFromDefaultSection<AzureKeyVaultOptions>();

        if (options is null)
        {
            return configuration;
        }

        bool isDevelopment = webHostEnvironment.IsDevelopment() || Debugger.IsAttached;
        return configuration.AddAzureKeyVault(options.Url, new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ExcludeInteractiveBrowserCredential = !isDevelopment,
            ExcludeManagedIdentityCredential = isDevelopment,
        }));
    }
}
