using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.Core;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class ResourceGroupTests
    {
        private readonly MockStackHelper _stackHelper;
        
        public ResourceGroupTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetName()
        {
            var resources = await _stackHelper.TestAsync();

            var resourceGroup = resources.OfType<ResourceGroup>().FirstOrDefault();
            Assert.NotNull(resourceGroup);
            
            var name = await resourceGroup.Name.GetValueAsync();
            Assert.NotNull(name);
            Assert.Equal("library-rg", name);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var resourceGroup = resources.OfType<ResourceGroup>().FirstOrDefault();
            Assert.NotNull(resourceGroup);
            
            var tags = await resourceGroup.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}