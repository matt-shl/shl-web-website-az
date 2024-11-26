using System.Text;
using DTNL.UmbracoCms.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;

namespace DTNL.UmbracoCms.SourceGenerators.ImageStylesHelperGenerator;

[Generator]
public class ImageStylesHelperGenerator : ISourceGenerator
{
    public const string GeneratorAttributeName = nameof(ImageStylesHelperGeneratorAttribute);

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(ctx => ctx.AddSource($"{GeneratorAttributeName}.generated.cs", EmbeddedResourceHelper.GetEmbeddedResource($"{nameof(ImageStylesHelperGenerator)}.{GeneratorAttributeName}.cs")));
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver(GeneratorAttributeName));
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver syntaxReceiver)
        {
            return;
        }

        foreach (TypeDeclarationSyntax candidateTypeNode in syntaxReceiver.Candidates)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);
            ITypeSymbol type = model.GetDeclaredSymbol(candidateTypeNode);

            try
            {
                (string name, SourceText result) = ExecuteForModel(context, type, model.SyntaxTree.FilePath);

                if (name is not null && result is not null)
                {
                    context.AddSource($"{name}.generated.cs", result);
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "SG0000",
                            "An exception occurred during code generation",
                            ex.Message,
                            nameof(ImageStylesHelperGenerator),
                            DiagnosticSeverity.Error,
                            true
                        ),
                        type?.Locations.FirstOrDefault()
                    )
                );
            }
        }
    }

    private static (string Name, SourceText Result) ExecuteForModel(GeneratorExecutionContext context, ITypeSymbol symbol, string filePath)
    {
        AttributeData attribute = symbol
            .GetAttributes($"{typeof(ImageStylesHelperGenerator).Namespace}.{GeneratorAttributeName}")
            .First();

        string directory = attribute.GetConstructorArgument<string>(0);

        string fileDirectory = Path.GetDirectoryName(filePath) ?? "";
        string directoryPath = DirectoryHelper.NormalizePath(Path.Combine(fileDirectory, directory));

        if (!Directory.Exists(directoryPath))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "SG0001",
                        "Unable to find directory",
                        "Could not find a part of the path '{0}'.",
                        nameof(ImageStylesHelperGenerator),
                        DiagnosticSeverity.Warning,
                        true
                    ),
                    symbol.Locations.FirstOrDefault(),
                    directoryPath
                )
            );

            return (null, null);
        }

        string symbolNamespace = symbol.ContainingNamespace.ToDisplayString();

        List<(string File, ImageStyle Style)> imageStyles = ParseJsonFiles(directoryPath);
        if (!imageStyles.Any())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "SG0001",
                        "Unable to find styles files",
                        "Could not find a part of the path '{0}'.",
                        nameof(ImageStylesHelperGenerator),
                        DiagnosticSeverity.Warning,
                        true
                    ),
                    symbol.Locations.FirstOrDefault(),
                    directoryPath
                )
            );

            return (null, null);
        }

        string stringResult = GenerateImageStylesHelperClass(symbolNamespace, imageStyles);

        SourceText result = SyntaxFactory.ParseCompilationUnit(stringResult)
            .NormalizeWhitespace()
            .GetText(Encoding.UTF8);

        return (symbol.Name, result);
    }

    private static string GenerateImageStylesHelperClass(string symbolNamespace, List<(string Name, ImageStyle Style)> imageStyles)
    {
        string dictionaryEntries = string.Join(
            Environment.NewLine,
            imageStyles.Select(entry =>
            {
                // Ensure correct use of int[] for Breakpoints
                IEnumerable<string> breakpoints = entry.Style.Breakpoints
                    .Select(bp => $@"new ImageCropping(""{bp.Key}"", {bp.Value[0]}, {bp.Value[1]})");

                return $@"            [""{entry.Name}""] = new[] {{ {string.Join(", ", breakpoints)} }},";
            })
        );

        return $@"
                #nullable enable
                using System.Collections.Generic;

                namespace {symbolNamespace}
                {{
                    public static partial class ImageStylesHelper
                    {{
                        private static readonly Dictionary<string, ImageCropping[]> _crops = 
                            new Dictionary<string, ImageCropping[]>()
                            {{
                                {dictionaryEntries}
                            }};

                        public static ImageCropping[] GetImageCroppings(string style) => 
                            _crops.TryGetValue(style, out ImageCropping[]? value) ? value : System.Array.Empty<ImageCropping>();
                    }}

                    public class ImageCropping
                    {{
                        public string Name {{ get; }}
                        public int Width {{ get; }}
                        public int Height {{ get; }}

                        public ImageCropping(string name, int width, int height)
                        {{
                            Name = name;
                            Width = width;
                            Height = height;
                        }}
                    }}
                }}";
    }

    private static List<(string Name, ImageStyle Style)> ParseJsonFiles(string dir)
    {
        string[] files = Directory.GetFiles(dir);
        return files
            .Select(file => (Path.GetFileNameWithoutExtension(file), GetImageStyle(file)))
            .Where<(string Name, ImageStyle Style)>(imgStyle => imgStyle.Style != null)
            .ToList();
    }

    private static ImageStyle GetImageStyle(string file)
    {
        string contents = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<ImageStyle>(contents);
    }
}
