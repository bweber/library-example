using System;
using System.Threading.Tasks;
using Dapper;
using Library.Common.Data.Entities;

namespace Library.AcceptanceTests.Utilities
{
    public static class SqlHelper
    {
        public static async Task<Author> GetAuthorById(Guid authorId)
        {
            using var connection = ConnectionFactory.GetConnection();
            
            return await connection.QuerySingleOrDefaultAsync<Author>(@"
                SELECT * FROM Library.dbo.Authors WHERE Id = @authorId",
                new { authorId }
            );
        }
        
        public static async Task CreateAuthor(Author author)
        {
            using var connection = ConnectionFactory.GetConnection();
            
            await connection.ExecuteAsync(@"
                INSERT INTO Library.dbo.Authors (Id, FirstName, LastName)
                VALUES (@Id, @FirstName, @LastName)",
                author
            );
        }
        
        public static async Task<Book> GetBookById(Guid bookId)
        {
            using var connection = ConnectionFactory.GetConnection();
            
            return await connection.QuerySingleOrDefaultAsync<Book>(@"
                SELECT * FROM Library.dbo.Books WHERE Id = @bookId",
                new { bookId }
            );
        }
        
        public static async Task CreateBook(Book book)
        {
            using var connection = ConnectionFactory.GetConnection();
            
            await connection.ExecuteAsync(@"
                INSERT INTO Library.dbo.Books (Id, AuthorId, Title, Subject)
                VALUES (@Id, @AuthorId, @Title, @Subject)",
                book
            );
        }
        
        // public static async Task CreateRecipe(object recipe)
        // {
        //     using var connection = ConnectionFactory.GetConnection();
        //     
        //     await connection.ExecuteAsync(
        //         @"INSERT INTO Integration.dbo.Recipes (Id, Name)
        //               VALUES (@Id, @Name)", 
        //         recipe);
        // }
        //
        // public static async Task CreateIngredient(object ingredient)
        // {
        //     using var connection = ConnectionFactory.GetConnection();
        //     
        //     await connection.ExecuteAsync(
        //         @"INSERT INTO Integration.dbo.Ingredients (Id, RecipeId, Name, Amount)
        //               VALUES (@Id, @RecipeId, @Name, @Amount)", 
        //         ingredient);
        // }
        //
        // public static async Task<dynamic> GetGreetingWithResponseById(Guid greetingId)
        // {
        //     using var connection = ConnectionFactory.GetConnection();
        //     
        //     return await connection.QuerySingleAsync(@"
        //         SELECT Id, Name, GreetingResponse 
        //         FROM Integration.dbo.Greetings 
        //         WHERE Id = @greetingId AND GreetingResponse IS NOT NULL",
        //         new { greetingId }
        //     );
        // }
        //
        // public static async Task CreateGreeting(object greeting)
        // {
        //     using var connection = ConnectionFactory.GetConnection();
        //     
        //     await connection.ExecuteAsync(
        //         @"INSERT INTO Integration.dbo.Greetings (Id, Name)
        //           VALUES (@Id, @Name)", 
        //         greeting);
        // }
    }
}