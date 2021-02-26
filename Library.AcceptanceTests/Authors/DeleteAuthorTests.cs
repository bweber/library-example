using System;
using System.Net;
using System.Threading.Tasks;
using Library.AcceptanceTests.Utilities;
using Xunit;

namespace Library.AcceptanceTests.Authors
{
    public class DeleteAuthorTests
    {
        [Fact]
        public async Task ShouldDeleteAuthor()
        {
            var author = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(author);
            
            var response = await ApiHelper.Delete($"authors/{author.Id}");
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var dbAuthor = await SqlHelper.GetAuthorById(author.Id);
            Assert.Null(dbAuthor);
        }
        
        [Fact]
        public async Task ShouldReturnNotFoundResponseIfAuthorDoesntExist()
        {
            var response = await ApiHelper.Delete($"authors/{Guid.NewGuid()}");
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}