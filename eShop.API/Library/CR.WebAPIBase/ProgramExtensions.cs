using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace CR.WebAPIBase;

public static class ProgramExtensions
{
    public static void ConfigureLogging(
        this WebApplicationBuilder builder,
        string queueName,
        string routingKey
    )
    {
        var environment = builder.Environment.EnvironmentName;
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Project", "JVF")
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithProperty("Service", $"{Assembly.GetEntryAssembly()?.GetName().Name}")
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Host.UseSerilog(logger);
    }
}