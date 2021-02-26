using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Library.AcceptanceTests.Utilities;
using Library.Authors.Models;
using Xunit;

namespace Library.AcceptanceTests.Authors
{
    public class CreateAuthorTests
    {
        [Fact]
        public async Task ShouldCreateAuthorWithoutId()
        {
            var model = ModelBuilder.GenerateAuthorModel();
            model.Id = null;
            
            var response = await ApiHelper.Post("authors", model);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var author = await response.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(author);
            Assert.NotNull(author.Id);
            Assert.Equal(model.FirstName, author.FirstName);
            Assert.Equal(model.LastName, author.LastName);

            var locationHeader = response.Headers.Location;
            Assert.NotNull(locationHeader);
            Assert.EndsWith($"authors/{author.Id}", locationHeader.AbsoluteUri);

            var dbAuthor = await SqlHelper.GetAuthorById(author.Id.Value);
            Assert.NotNull(dbAuthor);
            Assert.Equal(model.FirstName, dbAuthor.FirstName);
            Assert.Equal(model.LastName, dbAuthor.LastName);
        }
        
        [Fact]
        public async Task ShouldCreateAuthorWithId()
        {
            var model = ModelBuilder.GenerateAuthorModel();
            
            var response = await ApiHelper.Post("authors", model);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var author = await response.Content.ReadFromJsonAsync<AuthorModel>();
            Assert.NotNull(author);
            Assert.NotNull(author.Id);
            Assert.Equal(model.Id, author.Id);
            Assert.Equal(model.FirstName, author.FirstName);
            Assert.Equal(model.LastName, author.LastName);

            var locationHeader = response.Headers.Location;
            Assert.NotNull(locationHeader);
            Assert.EndsWith($"authors/{author.Id}", locationHeader.AbsoluteUri);

            var dbAuthor = await SqlHelper.GetAuthorById(author.Id.Value);
            Assert.NotNull(dbAuthor);
            Assert.Equal(model.FirstName, dbAuthor.FirstName);
            Assert.Equal(model.LastName, dbAuthor.LastName);
        }
        
        [Fact]
        public async Task ShouldReturnSeeOtherResponseIfAuthorWithIdExists()
        {
            var existing = ModelBuilder.GenerateAuthor();
            await SqlHelper.CreateAuthor(existing);

            var model = ModelBuilder.GenerateAuthorModel();
            model.Id = existing.Id;

            var response = await ApiHelper.Post("authors", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.SeeOther, response.StatusCode);
            
            var locationHeader = response.Headers.Location;
            Assert.NotNull(locationHeader);
            Assert.EndsWith($"authors/{existing.Id}", locationHeader.AbsoluteUri);

            var dbAuthor = await SqlHelper.GetAuthorById(existing.Id);
            Assert.NotNull(dbAuthor);
            Assert.Equal(existing.FirstName, dbAuthor.FirstName);
            Assert.Equal(existing.LastName, dbAuthor.LastName);
        }
        
        [Fact]
        public async Task ShouldValidateRequiredFields()
        {
            var model = new AuthorModel();
            
            var response = await ApiHelper.Post("authors", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorModel>();
            Assert.NotNull(errorResponse);
            Assert.Equal("first_name is required", errorResponse.Errors[nameof(AuthorModel.FirstName)].First());
            Assert.Equal("last_name is required", errorResponse.Errors[nameof(AuthorModel.LastName)].First());
        }
        
        [Fact]
        public async Task ShouldValidateMaxLength()
        {
            var model = new AuthorModel
            {
                FirstName = new string('A', 101),
                LastName = new string('A', 101)
            };
            
            var response = await ApiHelper.Post("authors", model);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorModel>();
            Assert.NotNull(errorResponse);
            Assert.Equal("first_name has a maximum length of 100", errorResponse.Errors[nameof(AuthorModel.FirstName)].First());
            Assert.Equal("last_name has a maximum length of 100", errorResponse.Errors[nameof(AuthorModel.LastName)].First());
        }
    }
}