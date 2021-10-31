using FluentValidation;
using Library.Books.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Books
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterBooks(this IServiceCollection services)
        {
            services.AddTransient<IValidator<BookModel>, BookModelValidator>();

            services.AddMediatR(typeof(DependencyConfiguration));

            return services;
        }
    }
}
