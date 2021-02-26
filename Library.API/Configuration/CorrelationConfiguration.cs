using CorrelationId.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Library.API.Configuration
{
    public static class CorrelationConfiguration
    {
        public static IServiceCollection ConfigureCorrelation(this IServiceCollection services)
        {
            services.AddCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.UpdateTraceIdentifier = true;
            }).WithTraceIdentifierProvider();

            return services;
        }
    }
}