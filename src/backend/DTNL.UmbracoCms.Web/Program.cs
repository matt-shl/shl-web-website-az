using System.Globalization;
using DTNL.UmbracoCms.Web;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

// Bootstrap logger
// (Only used until Umbraco sets up their own, based on the appsettings configuration)
Log.Logger = new LoggerConfiguration()
    .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
    .WriteTo.Async(configuration => configuration.Console(formatProvider: CultureInfo.InvariantCulture))
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up Umbraco CMS");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.ConfigureWebHost();
    builder.Configuration.ConfigureAppConfiguration(builder.Environment);
    builder.Services.ConfigureServices(builder.Environment, builder.Configuration);

    WebApplication app = builder.Build();

    await app.BootUmbracoAsync();

    app.ConfigureWebApplication();

    string environment = app.Environment.EnvironmentName;
    Log.Information("Running Umbraco CMS - {Environment}", environment);

    await app.RunAsync();

    Log.Information("Stopped Umbraco CMS - {Environment}", environment);
    return 0;
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly.");
    return 1;
}
finally
{
    // Explicitly flush followed by Delay.
    // This ensures that even if the application terminates, telemetry is sent to Application Insights.
    Log.CloseAndFlush();
    await Task.Delay(TimeSpan.FromMilliseconds(1000));
}
