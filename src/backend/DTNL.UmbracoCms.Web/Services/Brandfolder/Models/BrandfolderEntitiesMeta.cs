using System.Text.Json.Serialization;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderEntitiesMeta
{
    [JsonPropertyName("total_count")]
    public required int TotalCount { get; set; }
}
