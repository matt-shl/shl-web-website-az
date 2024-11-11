namespace DTNL.UmbracoCms.Web.Infrastructure.DataProtection;

/// <summary>
/// Data model used by <see cref="DataProtectionSqlRepository"/>.
/// </summary>
public class DataProtectionKey
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Gets or sets the friendly name.
    /// </summary>
    public string? FriendlyName { get; set; }

    /// <summary>
    /// Gets or sets the XML representation of the Key.
    /// </summary>
    public string? Xml { get; set; }
}
