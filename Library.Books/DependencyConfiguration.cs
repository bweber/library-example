using FluentValidation;
using Library.Books.Models;
using Library.Books.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Books
{
    public static class DependencyConfiguration
    {
        public static IServiceCollection RegisterBooks(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddTransient<IValidator<BookModel>, BookModelValidator>();
            
            return services;
        }
    }
}