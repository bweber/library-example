using System;
using System.Threading.Tasks;
using Polly;
using Xunit;
using Xunit.Sdk;

namespace Library.IntegrationTests.Utilities
{
    public static class VersionHelper
    {
        public static async Task WaitForVersion()
        {
            var latestVersion = Environment.GetEnvironmentVariable("GIT_HASH");

            await Policy
                .Handle<AssertActualExpectedException>()
                .WaitAndRetryAsync(60, _ => TimeSpan.FromSeconds(5)) // Wait 5 minutes
                .ExecuteAsync(async () =>
                {
                    var response = await ApiHelper.Get("version");
                    Assert.True(response.IsSuccessStatusCode);

                    var version = await response.Content.ReadAsStringAsync();

                    Assert.True(latestVersion == version, $"Expected version {latestVersion} does not match deployed version {version}");
                });
        }
    }
}