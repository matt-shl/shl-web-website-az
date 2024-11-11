using System.ComponentModel.DataAnnotations;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats;

public class AtsApiClientOptions
{
    [Required]
    public required string HostName { get; set; }

    [Required]
    public required string Path { get; set; }
}
