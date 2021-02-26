using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Library.AcceptanceTests.Utilities;
using Library.Books.Models;
using Xunit;

namespace Library.AcceptanceTests.Books
{
    public class GetBookByIdTests
    {
        [Fact]
        public async Task ShouldGetBookById()
        {
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);
            
            var existing = ModelBuilder.GenerateBook(author.Id);
            await SqlHelper.CreateBook(existing);
            
            var response = await ApiHelper.Get($"books/{existing.Id}");
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var book = await response.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(book);
            Assert.NotNull(book.Id);
            Assert.Equal(existing.AuthorId, book.AuthorId);
            Assert.Equal(existing.Title, book.Title);
            Assert.Equal(existing.Subject, book.Subject);
        }
        
        [Fact]
        public async Task ShouldReturnNotFoundResponseIfBookDoesntExist()
        {
            var response = await ApiHelper.Get($"books/{Guid.NewGuid()}");
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}