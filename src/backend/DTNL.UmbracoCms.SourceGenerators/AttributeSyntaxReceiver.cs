using DTNL.UmbracoCms.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DTNL.UmbracoCms.SourceGenerators;

internal class AttributeSyntaxReceiver : ISyntaxReceiver
{
    private readonly string[] _attributeNames;

    public AttributeSyntaxReceiver(params string[] attributeNames)
    {
        _attributeNames = attributeNames;
    }

    public List<TypeDeclarationSyntax> Candidates { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        // Type declarations only
        if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return;
        }

        // Partial types only, as we can't extend otherwise
        if (!typeDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }

        // Check if they have the attribute we are looking for
        IEnumerable<AttributeSyntax> foundAttributes = typeDeclarationSyntax.AttributeLists.SelectMany(attributeList => attributeList.Attributes);
        IEnumerable<string> foundAttributeNames = foundAttributes.Select(a => a.Name.ToString().Split('.').Last().EnsureEndsWith("Attribute"));
        if (!foundAttributeNames.Intersect(_attributeNames).Any())
        {
            return;
        }

        Candidates.Add(typeDeclarationSyntax);
    }
}
