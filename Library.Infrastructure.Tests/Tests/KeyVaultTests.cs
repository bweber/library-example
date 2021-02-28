using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.KeyVault;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class KeyVaultTests
    {
        private readonly MockStackHelper _stackHelper;

        public KeyVaultTests()
        {
            _stackHelper = new MockStackHelper();
        }

        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var resourceGroupName = await keyVault.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }

        [Fact]
        public async Task ShouldSetSkuName()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var skuName = await keyVault.SkuName.GetValueAsync();
            Assert.Equal(_stackHelper.Config.KeyVaultSku, skuName);
        }

        [Fact]
        public async Task ShouldSetPurgeProtection()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var purgeProtection = await keyVault.PurgeProtectionEnabled.GetValueAsync();
            Assert.True(purgeProtection);
        }

        [Fact]
        public async Task ShouldSet90DaySoftDeleteRetentionDays()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var softDeleteRetentionDays = await keyVault.SoftDeleteRetentionDays.GetValueAsync();
            Assert.Equal(90, softDeleteRetentionDays);
        }

        [Fact]
        public async Task ShouldSetDefaultAccessPolicy()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var accessPolicies = await keyVault.AccessPolicies.GetValueAsync();
            Assert.Single(accessPolicies);

            var accessPolicy = accessPolicies.First();
            Assert.Equal(_stackHelper.ClientConfig["tenantId"], accessPolicy.TenantId);
            Assert.Equal(_stackHelper.ClientConfig["objectId"], accessPolicy.ObjectId);
            Assert.True(accessPolicy.KeyPermissions.IsDefault);
            Assert.True(accessPolicy.CertificatePermissions.IsDefault);

            Assert.Equal(6, accessPolicy.SecretPermissions.Length);
            Assert.Contains("get", accessPolicy.SecretPermissions);
            Assert.Contains("list", accessPolicy.SecretPermissions);
            Assert.Contains("set", accessPolicy.SecretPermissions);
            Assert.Contains("delete", accessPolicy.SecretPermissions);
            Assert.Contains("recover", accessPolicy.SecretPermissions);
            Assert.Contains("purge", accessPolicy.SecretPermissions);
        }

        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var tags = await keyVault.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}