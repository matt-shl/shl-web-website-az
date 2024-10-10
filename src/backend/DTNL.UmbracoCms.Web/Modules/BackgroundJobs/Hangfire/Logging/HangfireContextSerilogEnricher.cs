using Hangfire.Server;
using Serilog.Core;
using Serilog.Events;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire.Logging;

/// <summary>
/// Hangfire Context Serilog Enricher.
/// </summary>
public class HangfireContextSerilogEnricher : ILogEventEnricher
{
    /// <inheritdoc />
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        PerformingContext? context = HangfirePerformingContextAccessor.Value;
        if (context == null)
        {
            return;
        }

        // Create property value with PerformContext and put as "PerformContext"
        LogEventProperty property = new("HangFireJob", new HangfireContextSerilogStructureValue(context));
        logEvent.AddOrUpdateProperty(property);
    }

    /// <summary>
    /// Hangfire Context Serilog Structure Value.
    /// </summary>
    public class HangfireContextSerilogStructureValue : StructureValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HangfireContextSerilogStructureValue"/> class.
        /// </summary>
        public HangfireContextSerilogStructureValue(PerformingContext performingContext)
            : base(CreateProperties(performingContext))
        {
            PerformingContext = performingContext;
        }

        internal PerformingContext PerformingContext { get; }

        private static List<LogEventProperty> CreateProperties(PerformingContext performingContext)
        {
            List<LogEventProperty> properties =
            [
                new("Id", new ScalarValue(performingContext.BackgroundJob.Id)),
                new("CreatedAt", new ScalarValue(performingContext.BackgroundJob.CreatedAt)),
            ];

            if (performingContext.BackgroundJob.Job != null)
            {
                properties.Add(new LogEventProperty("Type", new ScalarValue(performingContext.BackgroundJob.Job.Method.DeclaringType?.Name)));
                properties.Add(new LogEventProperty("Method", new ScalarValue(performingContext.BackgroundJob.Job.Method.Name)));
                properties.Add(new LogEventProperty("Arguments", new SequenceValue(performingContext.BackgroundJob.Job.Args.Select(GetScalarValue))));
            }

            return properties;
        }

        private static ScalarValue GetScalarValue(object? value)
        {
            if (value?.GetType().IsPrimitive == false)
            {
                value = value.ToString();
            }

            return new ScalarValue(value);
        }
    }
}
