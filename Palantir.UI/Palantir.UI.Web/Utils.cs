using Serilog;

namespace Palantir.UI.Web;

internal static class Utils
{
    internal static Microsoft.Extensions.Logging.ILogger ProduceLogger()
    {
        // Configuration system
        var Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq(Configuration["Horizon:Logs:Endpoint"] ?? "http://localhost:5341")
            .CreateLogger();
        AppDomain.CurrentDomain.ProcessExit += (_1, _2) => Log.CloseAndFlush();

        // Microsoft ILogger configuration
        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddSerilog();
            builder.AddFilter(Configuration["Horizon:Component"] ?? "Horizon",
                (LogLevel)Enum.Parse(typeof(LogLevel), Configuration["Horizon:Logs:LogLevel"] ?? "Debug"));
        });
        Microsoft.Extensions.Logging.ILogger logger = factory.CreateLogger(Configuration["Horizon:Component"] ?? "Horizon");

        return logger;
    }
}
