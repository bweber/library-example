using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.KeyVault;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class AppServiceTests
    {
        private readonly MockStackHelper _stackHelper;

        public AppServiceTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var resourceGroupName = await appService.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldSetLocationFromResourceGroup()
        {
            var resources = await _stackHelper.TestAsync();

            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var location = await appService.Location.GetValueAsync();
            Assert.NotNull(location);
            Assert.Equal("CentralUS", location);
        }
        
        [Fact]
        public async Task ShouldSetAppServicePlanId()
        {
            var resources = await _stackHelper.TestAsync();
            
            var plan = resources.OfType<Plan>().FirstOrDefault();
            Assert.NotNull(plan);
            
            var planId = await plan.Id.GetValueAsync();
            
            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var appServicePlanId = await appService.AppServicePlanId.GetValueAsync();
            Assert.NotNull(appServicePlanId);
            Assert.Equal(planId, appServicePlanId);
        }
        
        [Fact]
        public async Task ShouldBeHttpsOnly()
        {
            var resources = await _stackHelper.TestAsync();

            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var httpsOnly = await appService.HttpsOnly.GetValueAsync();
            Assert.NotNull(httpsOnly);
            Assert.True(httpsOnly);
        }
        
        [Fact]
        public async Task ShouldUseSystemAssignedIdentity()
        {
            var resources = await _stackHelper.TestAsync();

            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var identity = await appService.Identity.GetValueAsync();
            Assert.NotNull(identity);
            Assert.Equal("SystemAssigned", identity.Type);
        }
        
        [Fact]
        public async Task ShouldHaveAppSettingsSet()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var keyVaultName = await keyVault.Name.GetValueAsync();
            
            var appInsights = resources.OfType<Insights>().FirstOrDefault();
            Assert.NotNull(appInsights);

            var instrumentationKey = await appInsights.InstrumentationKey.GetValueAsync();
            
            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var appSettings = await appService.AppSettings.GetValueAsync();
            Assert.NotNull(appSettings);
            Assert.Equal(4, appSettings.Count);
            Assert.Equal("false", appSettings["WEBSITES_ENABLE_APP_SERVICE_STORAGE"]);
            Assert.Equal(_stackHelper.Config.AspnetEnvironment, appSettings["ASPNETCORE_ENVIRONMENT"]);
            Assert.Equal(instrumentationKey, appSettings["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            Assert.Equal(keyVaultName, appSettings["KeyVaultName"]);
        }
        
        [Fact]
        public async Task ShouldHaveSiteConfigSet()
        {
            var resources = await _stackHelper.TestAsync();
            
            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var siteConfig = await appService.SiteConfig.GetValueAsync();
            Assert.NotNull(siteConfig);
            Assert.Equal("DOCKER|microsoft/azure-appservices-go-quickstart", siteConfig.LinuxFxVersion);
            Assert.Equal("Disabled", siteConfig.FtpsState);
            Assert.Equal("/healthz", siteConfig.HealthCheckPath);
            Assert.False(siteConfig.RemoteDebuggingEnabled);
            Assert.Equal("1.2", siteConfig.MinTlsVersion);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var tags = await appService.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
        
        [Fact]
        public async Task ShouldHaveDefaultServiceLogs()
        {
            var resources = await _stackHelper.TestAsync();

            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var logs = await appService.Logs.GetValueAsync();
            Assert.NotNull(logs);
            Assert.NotNull(logs.HttpLogs);
            Assert.NotNull(logs.HttpLogs.FileSystem);
            Assert.Equal(14, logs.HttpLogs.FileSystem.RetentionInDays);
            Assert.Equal(35, logs.HttpLogs.FileSystem.RetentionInMb);
        }
        
        [Fact]
        public async Task ShouldHaveAnAppPolicyToAccessKeyVault()
        {
            var resources = await _stackHelper.TestAsync();

            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);

            var vaultId = await keyVault.Id.GetValueAsync();
            
            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var appServiceIdentity = await appService.Identity.GetValueAsync();
            
            var appAccessPolicy = resources.OfType<AccessPolicy>().FirstOrDefault();
            Assert.NotNull(appAccessPolicy);

            var keyVaultId = await appAccessPolicy.KeyVaultId.GetValueAsync();
            Assert.NotNull(keyVaultId);
            Assert.Equal(vaultId, keyVaultId);
            
            var tenantId = await appAccessPolicy.TenantId.GetValueAsync();
            Assert.NotNull(tenantId);
            Assert.Equal(_stackHelper.ClientConfig["tenantId"], tenantId);
            
            var principalId = await appAccessPolicy.ObjectId.GetValueAsync();
            Assert.NotNull(principalId);
            Assert.Equal(appServiceIdentity.PrincipalId, principalId);
            
            var secretPermissions = await appAccessPolicy.SecretPermissions.GetValueAsync();
            Assert.Equal(2, secretPermissions.Length);
            Assert.Contains("get", secretPermissions);
            Assert.Contains("list", secretPermissions);
        }
    }
}