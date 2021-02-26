using System;
using Library.Common.Data;
using Library.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Common
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterCommon(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AcceptanceOnlyAttribute>();

            services.AddDbContext<LibraryDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("LibraryDB"), providerOptions =>
                {
                    providerOptions.EnableRetryOnFailure(6, TimeSpan.FromSeconds(15), null);
                });
            });
            
            services.AddSingleton<Func<LibraryDBContext>>(serviceProvider => serviceProvider.GetRequiredService<LibraryDBContext>);
            
            return services;
        }
    }
}