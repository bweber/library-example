using Library.Authors;
using Library.Books;
using Library.Common;
using Library.Common.Extensions;
using Library.Common.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.API.Configuration
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            return services
                .RegisterLogging()
                .RegisterCommon(configuration)
                .RegisterAuthors()
                .RegisterBooks();
        }
    }
}
