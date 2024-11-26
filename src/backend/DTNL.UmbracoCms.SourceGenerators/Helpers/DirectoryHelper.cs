namespace DTNL.UmbracoCms.SourceGenerators.Helpers;

internal static class DirectoryHelper
{
    public static string NormalizePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
