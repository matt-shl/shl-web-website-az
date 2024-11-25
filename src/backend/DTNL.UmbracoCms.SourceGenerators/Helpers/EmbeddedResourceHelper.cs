using System.Reflection;

namespace DTNL.UmbracoCms.SourceGenerators.Helpers;

internal static class EmbeddedResourceHelper
{
    public static string GetEmbeddedResource(string resourceName)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        resourceName = $"{assembly.GetName().Name}.{resourceName}";

        Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"Resource '{resourceName}' could not be found", nameof(resourceName));
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
