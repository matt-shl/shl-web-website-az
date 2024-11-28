using System.Text.Json;

namespace DTNL.UmbracoCms.Web.Models.BrandfolderAssets;

public class BrandfolderAsset
{
    public required string Id { get; init; }

    public required string Url { get; init; }

    public string? Name { get; init; }

    public string? Description { get; init; }

    public static BrandfolderAsset? Create(string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<BrandfolderAsset>(value);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
