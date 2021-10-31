using FluentValidation;
using Library.Authors.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Authors
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterAuthors(this IServiceCollection services)
        {
            services.AddTransient<IValidator<AuthorModel>, AuthorModelValidator>();

            services.AddMediatR(typeof(DependencyConfiguration));

            return services;
        }
    }
}
