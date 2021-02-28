using FluentValidation;
using Library.Authors.Models;
using Library.Authors.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Authors
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterAuthors(this IServiceCollection services)
        {
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddTransient<IValidator<AuthorModel>, AuthorModelValidator>();
            
            return services;
        }
    }
}