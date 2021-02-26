using System;
using System.Net;
using System.Threading.Tasks;
using Library.AcceptanceTests.Utilities;
using Xunit;

namespace Library.AcceptanceTests.Books
{
    public class DeleteBookTests
    {
        [Fact]
        public async Task ShouldDeleteBook()
        {
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);
            
            var book = ModelBuilder.GenerateBook(author.Id);
            await SqlHelper.CreateBook(book);
            
            var response = await ApiHelper.Delete($"books/{book.Id}");
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var dbBook = await SqlHelper.GetBookById(book.Id);
            Assert.Null(dbBook);
        }
        
        [Fact]
        public async Task ShouldReturnNotFoundResponseIfBookDoesntExist()
        {
            var response = await ApiHelper.Delete($"books/{Guid.NewGuid()}");
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}