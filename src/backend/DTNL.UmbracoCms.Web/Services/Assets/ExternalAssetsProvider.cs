using Flurl.Http;

namespace DTNL.UmbracoCms.Web.Services.Assets;

public static class ExternalAssetsProvider
{
    public static async Task<string?> GetContent(string uri)
    {
        try
        {
            return await uri
                .WithTimeout(3)
                .GetStringAsync();
        }
        catch (FlurlHttpException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return string.Empty;
        }
        catch (FlurlHttpException)
        {
            return null;
        }
    }
}
