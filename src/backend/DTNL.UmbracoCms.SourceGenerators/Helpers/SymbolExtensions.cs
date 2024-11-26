using Microsoft.CodeAnalysis;

namespace DTNL.UmbracoCms.SourceGenerators.Helpers;

internal static class SymbolExtensions
{
    public static AttributeData GetAttribute(this ISymbol symbol, string fullName)
    {
        return symbol.GetAttributes(fullName).FirstOrDefault();
    }

    public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, string fullName)
    {
        return symbol.GetAttributes().Where(a => a.AttributeClass?.ToString() == fullName);
    }

    public static T GetConstructorArgument<T>(this AttributeData attributeData, int index)
    {
        return (T) attributeData.ConstructorArguments[index].Value;
    }

    public static T GetNamedArgument<T>(this AttributeData attributeData, string name)
    {
        return (T) attributeData.NamedArguments.FirstOrDefault(kp => kp.Key == name).Value.Value;
    }
}
