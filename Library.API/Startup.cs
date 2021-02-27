using System;
using System.Text.Json.Serialization;
using System.Threading;
using CorrelationId;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Library.API.Configuration;
using Library.Common.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services) => 
            services
                .AddApplicationInsightsTelemetry()
                .ConfigureHealthChecks(_configuration)
                .ConfigureDependencies(_configuration)
                .ConfigureCorrelation()
                .ConfigureCors()
                .ConfigureSwagger(_environment)
                .AddAutoMapper(typeof(Startup).Assembly)
                .AddControllers()
                .AddFluentValidation()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            Func<LibraryDBContext> createLibraryDBContext, ILogger<Startup> logger)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCorrelationId()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseCors()
                .UseSwaggerWithUI(env)
                .UseResponseCaching()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });

                    endpoints.MapControllers();
                });
            
            using var context = createLibraryDBContext();
            MigrateLibraryDB(context, logger, 20);
        }
        
        private static void MigrateLibraryDB(LibraryDBContext context, ILogger<Startup> logger, int retriesRemaining)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (SqlException e)
            {
                logger.LogWarning(e, "Error running migrations on the Library DB. Remaining Retries: {retriesRemaining}", retriesRemaining);
                if (retriesRemaining == 0)
                    throw;

                Thread.Sleep(3000);
                MigrateLibraryDB(context, logger, retriesRemaining - 1);
            }
        }
    }
}