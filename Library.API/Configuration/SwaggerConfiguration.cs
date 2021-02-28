using System.Collections.Generic;
using System.Linq;
using Library.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Library.API.Configuration
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IHostEnvironment environment)
        {
            if (!environment.IsProduction())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
                });
            }

            return services;
        }

        public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app, IHostEnvironment environment)
        {
            if (environment.IsProduction()) return app;
            
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                {
                    if (environment.IsAcceptance()) return;

                    var serverUrl = swaggerDoc.Servers.First();
                    serverUrl.Url = serverUrl.Url.Replace("http", "https");

                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        serverUrl
                    };
                });
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Library API v1"));

            return app;
        }
    }
}