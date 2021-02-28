using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.IntegrationTests.Utilities
{
    public static class ApiHelper
    {
        public static string BaseApiUrl = Environment.GetEnvironmentVariable("LIBRARY_API_URL");
        
        public static async Task<HttpResponseMessage> Get(string route)
        {
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            });
            
            return await httpClient.GetAsync($"{BaseApiUrl}/{route}");
        }
        
        public static async Task<HttpResponseMessage> Post(string route, object data)
        {
            var json = data == null ? "" : JsonSerializer.Serialize(data);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            });
            using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return await httpClient.PostAsync($"{BaseApiUrl}/{route}", stringContent);
        }
        
        public static async Task<HttpResponseMessage> Put(string route, object data)
        {
            var json = data == null ? "" : JsonSerializer.Serialize(data);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            });
            using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return await httpClient.PutAsync($"{BaseApiUrl}/{route}", stringContent);
        }
        
        public static async Task<HttpResponseMessage> Delete(string route)
        {
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            });

            return await httpClient.DeleteAsync($"{BaseApiUrl}/{route}");
        }
    }
}