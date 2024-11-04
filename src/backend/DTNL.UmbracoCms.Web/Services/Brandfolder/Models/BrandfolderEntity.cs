using System.Text.Json.Serialization;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderEntity
{
    public required string Id { get; set; }

    public required BrandfolderEntityAttributes Attributes { get; set; }
}

public class BrandfolderEntityAttributes
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    [JsonPropertyName("tagline")]
    public string? TagLine { get; set; }


    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }
}
