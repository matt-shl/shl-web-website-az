namespace DTNL.UmbracoCms.Web.Services.Assets;

public interface IAssetsProvider
{
    /// <summary>
    /// Retrieves the content of the file passed in the <paramref name="path"/>.
    /// </summary>
    /// <returns>The content of the file.</returns>
    /// <remarks>If the file was not found, an empty string will be returned instead. If an error occurred, null will be returned instead.</remarks>
    public Task<string?> GetContent(string path);
}
