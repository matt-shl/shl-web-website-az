using System.ComponentModel.DataAnnotations;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;

/// <summary>
///     The options used to set up Azure KeyVault client.
/// </summary>
public class AzureKeyVaultOptions
{
    /// <summary>
    ///     Url for the Key Vault instance
    /// </summary>
    [Required]
    public Uri Url { get; set; } = null!;
}
