namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;

public class DevelopmentOptions
{
    public List<string> AssetsFallbackDirectories { get; protected set; } = [];

    public Uri? AssetsFallbackUri { get; set; }

    public List<string> AssetsSubdirectories { get; protected set; } = [];

    public bool DeveloperExceptionPage { get; set; }
}
