using System.Linq;
using System.Net;
using System.Net.Http;
using Library.AcceptanceTests.Utilities;
using Xunit;

namespace Library.AcceptanceTests
{
    public class CorrelationIdTests
    {
        [Fact]
        public async void ShouldCreateCorrelationIdIfNotPassed()
        {
            using var httpClient = new HttpClient();
            
            var response = await httpClient.GetAsync($"{ApiHelper.BaseApiUrl}/healthz");
            var header = response.Headers.GetValues("X-Correlation-ID").FirstOrDefault();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(header);
        }
        
        [Fact]
        public async void ShouldGetSamePassedInCorrelationId()
        {
            var expectedCorrelationId = System.Guid.NewGuid().ToString();
            
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", expectedCorrelationId);
            
            var response = await httpClient.GetAsync($"{ApiHelper.BaseApiUrl}/healthz");
            var header = response.Headers.GetValues("X-Correlation-ID").FirstOrDefault();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(header);
            Assert.Equal(expectedCorrelationId, header);
        }
    }
}