using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DTNL.UmbracoCms.SourceGenerators.DiagnosticSuppressors;

/// <summary>
/// Suppresses the SA1003 diagnostic for cast operators.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CastOperatorTrailingSpaceDiagnosticSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor SuppressionDescriptor = new("SPSA1003", "SA1003", "Whitespace should be allowed after cast operators");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(SuppressionDescriptor);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode root = diagnostic.Location.SourceTree?.GetRoot().FindNode(diagnostic.Location.SourceSpan);

            if (root is CastExpressionSyntax or ArgumentSyntax { Expression: CastExpressionSyntax })
            {
                context.ReportSuppression(Suppression.Create(SuppressionDescriptor, diagnostic));
            }
        }
    }
}
