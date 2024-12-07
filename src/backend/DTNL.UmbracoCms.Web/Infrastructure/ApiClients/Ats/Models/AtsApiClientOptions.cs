using System.ComponentModel.DataAnnotations;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;

public class AtsApiClientOptions
{
    [Required]
    public required string HostName { get; set; }

    [Required]
    public required string Path { get; set; }

    [Required]
    public required string ExternalUrlFormat { get; set; }
}
