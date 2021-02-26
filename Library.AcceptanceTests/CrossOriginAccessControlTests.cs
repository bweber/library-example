using System.Linq;
using System.Net;
using System.Net.Http;
using Library.AcceptanceTests.Utilities;
using Xunit;

namespace Library.AcceptanceTests
{
    public class CrossOriginAccessControlTests
    {
        [Fact]
        public async void ShouldReturnWildCardAllowingAllUrlDomainOrigins()
        {
            const string expectedOriginsAllowed = "*";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Origin", "someOtherUrl");

            var response = await httpClient.GetAsync($"{ApiHelper.BaseApiUrl}/healthz");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var actualOriginAllowed = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            Assert.NotNull(actualOriginAllowed);
            Assert.Equal(expectedOriginsAllowed, actualOriginAllowed);
        }
    }
}