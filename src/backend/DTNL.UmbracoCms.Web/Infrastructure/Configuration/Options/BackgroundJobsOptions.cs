namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;

public class BackgroundJobsOptions
{
    public bool Enabled { get; set; } = true;

    public Dictionary<string, string> Schedule { get; set; } = new();
}
