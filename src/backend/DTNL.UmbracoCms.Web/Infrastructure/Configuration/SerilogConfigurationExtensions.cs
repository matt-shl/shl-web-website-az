using Serilog;
using Serilog.Configuration;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.Filters;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration;

public static class SerilogConfigurationExtensions
{
    /// <summary>
    /// Enrich logger output with a destructured object containing exception's public properties.
    /// </summary>
    /// <remarks>Necessary to allow AppSettings based configuration.</remarks>
    public static LoggerConfiguration WithCustomExceptionDetails(
        this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration,
        string rootName = "ExceptionDetail",
        int destructuringDepth = 10,
        bool disableReflectionBasedDestructurer = false,
        string filter = "IgnoreStackTraceAndTargetSiteExceptionFilter",
        string[]? destructurers = null
    )
    {
        DestructuringOptionsBuilder options = new DestructuringOptionsBuilder()
            .WithRootName(rootName)
            .WithDestructuringDepth(destructuringDepth);

        if (disableReflectionBasedDestructurer)
        {
            options.WithoutReflectionBasedDestructurer();
        }

        foreach (string destructurer in destructurers ?? ["DefaultDestructurers"])
        {
            switch (destructurer)
            {
                case "DefaultDestructurers":
                    options.WithDefaultDestructurers();
                    break;
                default:
                    options.WithDestructurers([CreateInstance<IExceptionDestructurer>(destructurer)]);
                    break;
            }
        }

        switch (filter)
        {
            case "IgnoreStackTraceAndTargetSiteExceptionFilter":
                options.WithIgnoreStackTraceAndTargetSiteExceptionFilter();
                break;
            default:
                options.WithFilter(CreateInstance<IExceptionPropertyFilter>(filter));
                break;
        }

        return loggerEnrichmentConfiguration.With(new ExceptionEnricher(options));
    }

    private static T CreateInstance<T>(string typeName)
    {
        Type type = Type.GetType(typeName) ?? throw new InvalidOperationException($"Unable to find type '{typeName}'");
        return (T) (Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Unable to instantiate type '{typeName}'"));
    }
}
