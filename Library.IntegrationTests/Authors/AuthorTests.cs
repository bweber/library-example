using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Library.Authors.Models;
using Library.IntegrationTests.Utilities;
using Xunit;

namespace Library.IntegrationTests.Authors
{
    public class AuthorTests
    {
        [Fact]
        public async Task ShouldPerformFullAuthorLifecycle()
        {
            await VersionHelper.WaitForVersion();
            
            var model = new AuthorModel
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString()
            };
            
            var insertResponse = await ApiHelper.Post("authors", model);
            Assert.True(insertResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, insertResponse.StatusCode);
            
            var insertedAuthor = await insertResponse.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(insertedAuthor);
            Assert.Equal(model.FirstName, insertedAuthor.FirstName);
            Assert.Equal(model.LastName, insertedAuthor.LastName);

            var getResponse = await ApiHelper.Get($"authors/{insertedAuthor.Id}");
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var retrievedAuthor = await insertResponse.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(retrievedAuthor);
            Assert.Equal(insertedAuthor.Id, retrievedAuthor.Id);
            Assert.Equal(model.FirstName, retrievedAuthor.FirstName);
            Assert.Equal(model.LastName, retrievedAuthor.LastName);

            var updatedModel = new AuthorModel
            {
                Id = retrievedAuthor.Id,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString()
            };
            
            var updateResponse = await ApiHelper.Put($"authors/{updatedModel.Id}", updatedModel);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            
            var updatedAuthor = await updateResponse.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(updatedAuthor);
            Assert.Equal(updatedModel.Id, updatedAuthor.Id);
            Assert.Equal(updatedModel.FirstName, updatedAuthor.FirstName);
            Assert.Equal(updatedModel.LastName, updatedAuthor.LastName);
            
            var deletedResponse = await ApiHelper.Delete($"authors/{updatedAuthor.Id}");
            Assert.True(deletedResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NoContent, deletedResponse.StatusCode);
        }
    }
}