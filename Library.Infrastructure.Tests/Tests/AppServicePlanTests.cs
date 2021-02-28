using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.AppService;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class AppServicePlanTests
    {
        private readonly MockStackHelper _stackHelper;

        public AppServicePlanTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);

            var resourceGroupName = await plan.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldSetLocationFromResourceGroup()
        {
            var resources = await _stackHelper.TestAsync();

            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);

            var location = await plan.Location.GetValueAsync();
            Assert.NotNull(location);
            Assert.Equal("CentralUS", location);
        }
        
        [Fact]
        public async Task ShouldSetLinuxKind()
        {
            var resources = await _stackHelper.TestAsync();

            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);

            var kind = await plan.Kind.GetValueAsync();
            Assert.NotNull(kind);
            Assert.Equal("Linux", kind);
        }
        
        [Fact]
        public async Task ShouldBeReserved()
        {
            var resources = await _stackHelper.TestAsync();

            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);

            var reserved = await plan.Reserved.GetValueAsync();
            Assert.NotNull(reserved);
            Assert.True(reserved);
        }
        
        [Fact]
        public async Task ShouldSetPlanSku()
        {
            var resources = await _stackHelper.TestAsync();

            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);

            var sku = await plan.Sku.GetValueAsync();
            Assert.NotNull(sku);
            
            Assert.Equal(_stackHelper.Config.AppServicePlanTier, sku.Tier);
            Assert.Equal(_stackHelper.Config.AppServicePlanSize, sku.Size);
            Assert.Equal(_stackHelper.Config.AppServiceCapacity, sku.Capacity);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);

            var tags = await plan.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}