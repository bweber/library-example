using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.AppInsights;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class AppInsightsTests
    {
        private readonly MockStackHelper _stackHelper;

        public AppInsightsTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var appInsights = resources.OfType<Insights>().FirstOrDefault();
            Assert.NotNull(appInsights);

            var resourceGroupName = await appInsights.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldSetLocationFromResourceGroup()
        {
            var resources = await _stackHelper.TestAsync();

            var appInsights = resources.OfType<Insights>().FirstOrDefault();
            Assert.NotNull(appInsights);

            var location = await appInsights.Location.GetValueAsync();
            Assert.NotNull(location);
            Assert.Equal("CentralUS", location);
        }
        
        [Fact]
        public async Task ShouldSetWebApplicationType()
        {
            var resources = await _stackHelper.TestAsync();

            var appInsights = resources.OfType<Insights>().FirstOrDefault();
            Assert.NotNull(appInsights);

            var type = await appInsights.ApplicationType.GetValueAsync();
            Assert.NotNull(type);
            Assert.Equal("web", type);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var appInsights = resources.OfType<Insights>().FirstOrDefault();
            Assert.NotNull(appInsights);

            var tags = await appInsights.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}