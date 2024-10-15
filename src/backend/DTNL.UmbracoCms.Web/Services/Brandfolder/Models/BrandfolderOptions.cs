using System.ComponentModel.DataAnnotations;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderOptions
{
    [Required]
    public required string ApiKey { get; set; }
}
