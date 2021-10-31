using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.IntegrationTests.Utilities
{
    public static class ApiHelper
    {
        public const string BaseApiUrl = "https://library-app.azurewebsites.net";

        public static async Task<HttpResponseMessage> Get(string route)
        {
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            })
            {
                BaseAddress = new Uri(BaseApiUrl)
            };

            return await httpClient.GetAsync(route);
        }

        public static async Task<HttpResponseMessage> Post(string route, object data)
        {
            var json = data == null ? "" : JsonSerializer.Serialize(data);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            })
            {
                BaseAddress = new Uri(BaseApiUrl)
            };

            using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return await httpClient.PostAsync(route, stringContent);
        }

        public static async Task<HttpResponseMessage> Put(string route, object data)
        {
            var json = data == null ? "" : JsonSerializer.Serialize(data);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            })
            {
                BaseAddress = new Uri(BaseApiUrl)
            };

            using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return await httpClient.PutAsync(route, stringContent);
        }

        public static async Task<HttpResponseMessage> Delete(string route)
        {
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            })
            {
                BaseAddress = new Uri(BaseApiUrl)
            };

            return await httpClient.DeleteAsync(route);
        }
    }
}
