using Hangfire.Console;
using Hangfire.Server;
using Serilog.Core;
using Serilog.Events;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire.Logging;

/// <summary>
/// Hangfire console Serilog sink.
/// </summary>
public class HangfireConsoleSerilogSink : ILogEventSink
{
    private readonly IFormatProvider? _formatProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="HangfireConsoleSerilogSink"/> class.
    /// </summary>
    public HangfireConsoleSerilogSink(IFormatProvider? formatProvider)
    {
        _formatProvider = formatProvider;
    }

    /// <inheritdoc />
    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("HangFireJob", out LogEventPropertyValue? logEventPerformContext))
        {
            // Get the object reference from our custom property
            PerformingContext? performContext = (logEventPerformContext as HangfireContextSerilogEnricher.HangfireContextSerilogStructureValue)?.PerformingContext;

            // And write the line on it
            performContext?.WriteLine(GetConsoleColor(logEvent.Level), logEvent.RenderMessage(_formatProvider));
        }
    }

    private static ConsoleTextColor GetConsoleColor(LogEventLevel logLevel)
    {
        return logLevel switch
        {
            LogEventLevel.Fatal => ConsoleTextColor.Red,
            LogEventLevel.Error => ConsoleTextColor.Red,
            LogEventLevel.Warning => ConsoleTextColor.Yellow,
            LogEventLevel.Information => ConsoleTextColor.White,
            LogEventLevel.Debug => ConsoleTextColor.Gray,
            LogEventLevel.Verbose => ConsoleTextColor.DarkGray,
            _ => ConsoleTextColor.White,
        };
    }
}
