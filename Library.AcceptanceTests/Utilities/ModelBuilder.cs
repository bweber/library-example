using System;
using Bogus;
using Library.Authors.Models;
using Library.Books.Models;
using Library.Common.Data.Entities;

namespace Library.AcceptanceTests.Utilities
{
    public static class ModelBuilder
    {
        public static AuthorModel GenerateAuthorModel()
        {
            return new Faker<AuthorModel>()
                .RuleFor(a => a.Id, (_, _) => Guid.NewGuid())
                .RuleFor(a => a.FirstName, (f, _) => f.Name.FirstName())
                .RuleFor(a => a.LastName, (f, _) => f.Name.LastName());
        }
        
        public static Author GenerateAuthor()
        {
            return new Faker<Author>()
                .RuleFor(a => a.Id, (_, _) => Guid.NewGuid())
                .RuleFor(a => a.FirstName, (f, _) => f.Name.FirstName())
                .RuleFor(a => a.LastName, (f, _) => f.Name.LastName());
        }
        
        public static BookModel GenerateBookModel(Guid authorId)
        {
            return new Faker<BookModel>()
                .RuleFor(a => a.Id, (_, _) => Guid.NewGuid())
                .RuleFor(a => a.AuthorId, (_, _) => authorId)
                .RuleFor(a => a.Title, (f, _) => f.Random.Word())
                .RuleFor(a => a.Subject, (f, _) => f.Random.Word());
        }
        
        public static Book GenerateBook(Guid authorId)
        {
            return new Faker<Book>()
                .RuleFor(a => a.Id, (_, _) => Guid.NewGuid())
                .RuleFor(a => a.AuthorId, (_, _) => authorId)
                .RuleFor(a => a.Title, (f, _) => f.Random.Word())
                .RuleFor(a => a.Subject, (f, _) => f.Random.Word());
        }
    }
}