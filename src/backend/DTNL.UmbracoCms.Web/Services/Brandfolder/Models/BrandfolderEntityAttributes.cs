using System.Text.Json.Serialization;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderEntityAttributes
{
    public string? FileName { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [JsonPropertyName("tagline")]
    public string? TagLine { get; set; }

    [JsonPropertyName("cdn_url")]
    public required string CdnUrl { get; set; }
}
