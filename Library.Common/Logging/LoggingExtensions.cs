using CorrelationId;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

namespace Library.Common.Logging
{
    public static class LoggingExtensions
    {
        public static IHostBuilder ConfigureLogging(this IHostBuilder host)
        {
            return host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithCorrelationIdHeader(CorrelationIdOptions.DefaultHeader)
                .Enrich.With(services.GetService<CustomHttpContextEnricher>())
                .WriteTo.Async(a =>
                    a.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Events))
                .WriteTo.Async(a => a.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code))
            );
        }

        public static IServiceCollection RegisterLogging(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddTransient<CustomHttpContextEnricher>();
            services.AddSingleton<ITelemetryInitializer, HttpContextTelemetryInitializer>();

            return services;
        }
    }
}
