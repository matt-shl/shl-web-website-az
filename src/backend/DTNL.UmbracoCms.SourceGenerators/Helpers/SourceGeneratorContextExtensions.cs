using Microsoft.CodeAnalysis;

namespace DTNL.UmbracoCms.SourceGenerators.Helpers;

internal static class SourceGeneratorContextExtensions
{
    public static string GetMSBuildProperty(
        this GeneratorExecutionContext context,
        string name,
        string defaultValue = "")
    {
        return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out string value)
            ? value
            : defaultValue;
    }
}
