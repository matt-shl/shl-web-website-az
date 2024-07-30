using Microsoft.Extensions.FileProviders;

namespace DTNL.UmbracoCms.Web.Services.Assets;

public class FileSystemAssetsProvider : IAssetsProvider
{
    private readonly ILogger<FileSystemAssetsProvider> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileSystemAssetsProvider(ILogger<FileSystemAssetsProvider> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string?> GetContent(string path)
    {
        try
        {
            IFileInfo? fileInfo = _webHostEnvironment.WebRootFileProvider.GetFileInfo(path);
            if (!fileInfo.Exists)
            {
                return string.Empty;
            }

            await using Stream stream = fileInfo.CreateReadStream();
            StreamReader streamReader = new(stream, leaveOpen: true);

            return await streamReader.ReadToEndAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while retrieving asset '{Path}' from disk.", path);
            return null;
        }
    }
}
