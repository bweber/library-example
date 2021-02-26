using Library.Authors;
using Library.Books;
using Library.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.API.Configuration
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            
            return services.RegisterCommon(configuration)
                .RegisterAuthors()
                .RegisterBooks();
        }
    }
}