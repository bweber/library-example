using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Library.AcceptanceTests.Utilities;
using Library.Authors.Models;
using Xunit;

namespace Library.AcceptanceTests.Authors
{
    public class GetAuthorByIdTests
    {
        [Fact]
        public async Task ShouldGetAuthorById()
        {
            var existing = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(existing);
            
            var response = await ApiHelper.Get($"authors/{existing.Id}");
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var author = await response.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(author);
            Assert.NotNull(author.Id);
            Assert.Equal(existing.FirstName, author.FirstName);
            Assert.Equal(existing.LastName, author.LastName);
        }
        
        [Fact]
        public async Task ShouldReturnNotFoundResponseIfAuthorDoesntExist()
        {
            var response = await ApiHelper.Get($"authors/{Guid.NewGuid()}");
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}