using System.Text.Json;
using Flurl;

namespace DTNL.UmbracoCms.Web.Models.BrandfolderAssets;

public class BrandfolderAttachment
{
    public required string Id { get; init; }

    public string? FileName { get; init; }

    public required string Url { get; init; }

    public string? AssetId { get; set; }

    public string? AssetName { get; set; }

    public string? AssetDescription { get; set; }

    public static BrandfolderAttachment? Create(string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<BrandfolderAttachment>(value);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string? GetAssetUrl(string? value)
    {
        return Create(value)?.Url.RemoveQuery();
    }
}
