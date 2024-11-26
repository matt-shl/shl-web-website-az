using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DTNL.UmbracoCms.SourceGenerators.DiagnosticSuppressors;

/// <summary>
/// Suppresses the SA1313 diagnostic for discard parameters.
/// These must be used for cases where a parameter is no longer needed but is part of a shipped public API.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscardParameterDiagnosticSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor SuppressionDescriptor = new("SPSA1313", "SA1313", "Discard parameter '_' should be allowed");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(SuppressionDescriptor);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode root = diagnostic.Location.SourceTree?.GetRoot().FindNode(diagnostic.Location.SourceSpan);

            if (root is ParameterSyntax { Identifier.ValueText: "_" or "__" })
            {
                context.ReportSuppression(Suppression.Create(SuppressionDescriptor, diagnostic));
            }
        }
    }
}
