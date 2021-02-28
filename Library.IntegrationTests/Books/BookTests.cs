using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Library.Authors.Models;
using Library.Books.Models;
using Library.IntegrationTests.Utilities;
using Xunit;

namespace Library.IntegrationTests.Books
{
    public class BookTests
    {
        [Fact]
        public async Task ShouldPerformFullBookLifecycle()
        {
            await VersionHelper.WaitForVersion();
            
            var authorModel = new AuthorModel
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString()
            };
            
            var authorResponse = await ApiHelper.Post("authors", authorModel);
            Assert.True(authorResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, authorResponse.StatusCode);
            
            var author = await authorResponse.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(author);
            
            var model = new BookModel
            {
                Title = Guid.NewGuid().ToString(),
                Subject = Guid.NewGuid().ToString(),
                AuthorId = author.Id
            };
            
            var insertResponse = await ApiHelper.Post("books", model);
            Assert.True(insertResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, insertResponse.StatusCode);
            
            var insertedBook = await insertResponse.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(insertedBook);
            Assert.Equal(model.Title, insertedBook.Title);
            Assert.Equal(model.Subject, insertedBook.Subject);
            Assert.Equal(author.Id, insertedBook.AuthorId);

            var getResponse = await ApiHelper.Get($"books/{insertedBook.Id}");
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var retrievedBook = await insertResponse.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(retrievedBook);
            Assert.Equal(insertedBook.Id, retrievedBook.Id);
            Assert.Equal(model.Title, retrievedBook.Title);
            Assert.Equal(model.Subject, retrievedBook.Subject);
            Assert.Equal(author.Id, retrievedBook.AuthorId);
            
            var newAuthorResponse = await ApiHelper.Post("authors", authorModel);
            Assert.True(newAuthorResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, newAuthorResponse.StatusCode);
            
            var newAuthor = await newAuthorResponse.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(newAuthor);
            
            var updatedModel = new BookModel
            {
                Id = retrievedBook.Id,
                Title = Guid.NewGuid().ToString(),
                Subject = Guid.NewGuid().ToString(),
                AuthorId = newAuthor.Id
            };
            
            var updateResponse = await ApiHelper.Put($"books/{updatedModel.Id}", updatedModel);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            
            var updatedBook = await updateResponse.Content.ReadFromJsonAsync<BookModel>();
            Assert.NotNull(updatedBook);
            Assert.Equal(updatedModel.Id, updatedBook.Id);
            Assert.Equal(updatedModel.Title, updatedBook.Title);
            Assert.Equal(updatedModel.Subject, updatedBook.Subject);
            Assert.Equal(newAuthor.Id, updatedBook.AuthorId);
            
            var deletedResponse = await ApiHelper.Delete($"books/{updatedBook.Id}");
            Assert.True(deletedResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NoContent, deletedResponse.StatusCode);
        }
    }
}