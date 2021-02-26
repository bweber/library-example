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
    public class CreateBookTests
    {
        [Fact]
        public async Task ShouldCreateBookWithoutId()
        {
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);

            var model = ModelBuilder.GenerateBookModel(author.Id);
            model.Id = null;
            
            var response = await ApiHelper.Post("books", model);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var book = await response.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(book);
            Assert.NotNull(book.Id);
            Assert.Equal(model.AuthorId, book.AuthorId);
            Assert.Equal(model.Title, book.Title);
            Assert.Equal(model.Subject, book.Subject);

            var locationHeader = response.Headers.Location;
            Assert.NotNull(locationHeader);
            Assert.EndsWith($"books/{book.Id}", locationHeader.AbsoluteUri);

            var dbBook = await SqlHelper.GetBookById(book.Id.Value);
            Assert.NotNull(dbBook);
            Assert.Equal(model.AuthorId, dbBook.AuthorId);
            Assert.Equal(model.Title, dbBook.Title);
            Assert.Equal(model.Subject, dbBook.Subject);
        }
        
        [Fact]
        public async Task ShouldCreateBookWithId()
        {
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);

            var model = ModelBuilder.GenerateBookModel(author.Id);
            
            var response = await ApiHelper.Post("books", model);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var book = await response.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(book);
            Assert.NotNull(book.Id);
            Assert.Equal(model.Id, book.Id);
            Assert.Equal(model.AuthorId, book.AuthorId);
            Assert.Equal(model.Title, book.Title);
            Assert.Equal(model.Subject, book.Subject);

            var locationHeader = response.Headers.Location;
            Assert.NotNull(locationHeader);
            Assert.EndsWith($"books/{book.Id}", locationHeader.AbsoluteUri);

            var dbBook = await SqlHelper.GetBookById(book.Id.Value);
            Assert.NotNull(dbBook);
            Assert.Equal(model.AuthorId, dbBook.AuthorId);
            Assert.Equal(model.Title, dbBook.Title);
            Assert.Equal(model.Subject, dbBook.Subject);
        }
        
        [Fact]
        public async Task ShouldReturnSeeOtherResponseIfBookWithIdExists()
        {
            var existingAuthor = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(existingAuthor);

            var existing = ModelBuilder.GenerateBook(existingAuthor.Id);
            await SqlHelper.CreateBook(existing);

            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);
            
            var model = ModelBuilder.GenerateBookModel(author.Id);
            model.Id = existing.Id;

            var response = await ApiHelper.Post("books", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.SeeOther, response.StatusCode);
            
            var locationHeader = response.Headers.Location;
            Assert.NotNull(locationHeader);
            Assert.EndsWith($"books/{existing.Id}", locationHeader.AbsoluteUri);

            var dbBook = await SqlHelper.GetBookById(existing.Id);
            Assert.NotNull(dbBook);
            Assert.Equal(existing.AuthorId, dbBook.AuthorId);
            Assert.Equal(existing.Title, dbBook.Title);
            Assert.Equal(existing.Subject, dbBook.Subject);
        }
        
        [Fact]
        public async Task ShouldValidateRequiredFields()
        {
            var model = new BookModel();
            
            var response = await ApiHelper.Post("books", model);
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
            
            var response = await ApiHelper.Post("books", model);
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
            
            var response = await ApiHelper.Post("books", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorModel>();
            Assert.NotNull(errorResponse);
            Assert.Equal("author_id does not reference an existing author", errorResponse.Errors[nameof(BookModel.AuthorId)].First());
        }
    }
}