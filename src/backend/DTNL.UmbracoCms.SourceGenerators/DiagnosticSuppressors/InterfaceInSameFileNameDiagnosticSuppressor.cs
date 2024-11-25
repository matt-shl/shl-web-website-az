using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DTNL.UmbracoCms.SourceGenerators.DiagnosticSuppressors;

/// <summary>
/// Suppresses the SA1649 diagnostic for files with both an interface and implementation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InterfaceInSameFileNameDiagnosticSuppressor : DiagnosticSuppressor
{
    public const SyntaxKind FileScopedNamespaceDeclaration = (SyntaxKind) 8845;
    private static readonly SuppressionDescriptor SuppressionDescriptor = new("SPSA1649", "SA1649", "Interface and implementation in the same file should be allowed");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(SuppressionDescriptor);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            string fileName = Path.GetFileName(diagnostic.Location.SourceTree?.FilePath);
            string expectedFileName = diagnostic.Properties.GetValueOrDefault("ExpectedFileName", null);

            // Check if the actualFileName is just the expectedFileName minus the I interface prefix
            if (fileName == null || expectedFileName != $"I{fileName}")
            {
                continue;
            }

            // Get all type declarations
            SyntaxNode rootNode = diagnostic.Location.SourceTree.GetRoot();
            List<TypeDeclarationSyntax> typeDeclarations = rootNode
                    .DescendantNodes(descendIntoChildren: node =>
                           node.IsKind(SyntaxKind.CompilationUnit)
                        || node.IsKind(SyntaxKind.NamespaceDeclaration)
                        || node.IsKind(FileScopedNamespaceDeclaration))
                    .OfType<TypeDeclarationSyntax>()
                    .ToList();

            // Get the name of the first declared type (This is what should have been used for the expectedFileName)
            string firstTypeName = typeDeclarations.FirstOrDefault()?.Identifier.Text;

            // Check if there's any other type that matches the firstTypeName without the I interface prefix
            if (typeDeclarations.Any(node => $"I{node.Identifier.Text}" == firstTypeName))
            {
                context.ReportSuppression(Suppression.Create(SuppressionDescriptor, diagnostic));
            }
        }
    }
}
