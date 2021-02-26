using System;
using System.Net;
using System.Net.Http;
using Library.AcceptanceTests.Utilities;
using Xunit;

namespace Library.AcceptanceTests
{
    public class VersionEndpointTests
    {
        [Fact]
        public async void ShouldReturnAppVersion()
        {
            var gitHash = Environment.GetEnvironmentVariable("GIT_HASH");
            
            using var httpClient = new HttpClient();
            
            var response = await httpClient.GetAsync($"{ApiHelper.BaseApiUrl}/version");
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.NotNull(responseBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(gitHash, responseBody);
        }
    }
}