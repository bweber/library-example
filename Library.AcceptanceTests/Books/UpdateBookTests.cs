using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Library.AcceptanceTests.Utilities;
using Library.Books.Models;
using Xunit;

namespace Library.AcceptanceTests.Books
{
    public class UpdateBookTests
    {
        [Fact]
        public async Task ShouldUpdateBook()
        {
            var existingAuthor = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(existingAuthor);
            
            var existing = ModelBuilder.GenerateBook(existingAuthor.Id);
            await SqlHelper.CreateBook(existing);
            
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);
            
            var model = ModelBuilder.GenerateBookModel(author.Id);
            model.Id = existing.Id;
            
            var response = await ApiHelper.Put($"books/{model.Id}", model);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var book = await response.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(book);
            Assert.NotNull(book.Id);
            Assert.Equal(existing.Id, book.Id);
            Assert.Equal(author.Id, book.AuthorId);
            Assert.Equal(model.Title, book.Title);
            Assert.Equal(model.Subject, book.Subject);

            var dbBook = await SqlHelper.GetBookById(book.Id.Value);
            Assert.NotNull(dbBook);
            Assert.Equal(model.AuthorId, dbBook.AuthorId);
            Assert.Equal(model.Title, dbBook.Title);
            Assert.Equal(model.Subject, dbBook.Subject);
        }
        
        [Fact]
        public async Task ShouldReturnNotFoundResponseIfBookDoesntExist()
        {
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);

            var model = ModelBuilder.GenerateBookModel(author.Id);

            var response = await ApiHelper.Put($"books/{model.Id}", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldValidateRequiredFields()
        {
            var model = new BookModel();
            
            var response = await ApiHelper.Put($"books/{Guid.NewGuid()}", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorModel>();
            Assert.NotNull(errorResponse);
            Assert.Equal("author_id is required", errorResponse.Errors[nameof(BookModel.AuthorId)].First());
            Assert.Equal("title is required", errorResponse.Errors[nameof(BookModel.Title)].First());
            Assert.Equal("subject is required", errorResponse.Errors[nameof(BookModel.Subject)].First());
        }
        
        [Fact]
        public async Task ShouldValidateMaxLength()
        {
            var model = new BookModel
            {
                Title = new string('A', 251),
                Subject = new string('A', 251)
            };
            
            var response = await ApiHelper.Put($"books/{Guid.NewGuid()}", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorModel>();
            Assert.NotNull(errorResponse);
            Assert.Equal("title has a maximum length of 250", errorResponse.Errors[nameof(BookModel.Title)].First());
            Assert.Equal("subject has a maximum length of 250", errorResponse.Errors[nameof(BookModel.Subject)].First());
        }
        
        [Fact]
        public async Task ShouldValidateAuthor()
        {
            var model = ModelBuilder.GenerateBookModel(Guid.NewGuid());
            
            var response = await ApiHelper.Put($"books/{Guid.NewGuid()}", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorModel>();
            Assert.NotNull(errorResponse);
            Assert.Equal("author_id does not reference an existing author", errorResponse.Errors[nameof(BookModel.AuthorId)].First());
        }
    }
}