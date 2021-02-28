using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.AppService;
using Pulumi.Azure.Sql;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class SqlActiveDirectoryAdministratorTests
    {
        private readonly MockStackHelper _stackHelper;

        public SqlActiveDirectoryAdministratorTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var database = resources.OfType<ActiveDirectoryAdministrator>().FirstOrDefault();
            Assert.NotNull(database);

            var resourceGroupName = await database.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldSetTenantId()
        {
            var resources = await _stackHelper.TestAsync();

            var adAdmin = resources.OfType<ActiveDirectoryAdministrator>().FirstOrDefault();
            Assert.NotNull(adAdmin);

            var resourceGroupName = await adAdmin.TenantId.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal(_stackHelper.ClientConfig["tenantId"], resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldHaveAnAppPolicyToAccessKeyVault()
        {
            var resources = await _stackHelper.TestAsync();
            
            var appService = resources.OfType<AppService>().FirstOrDefault();
            Assert.NotNull(appService);

            var appServiceIdentity = await appService.Identity.GetValueAsync();
            
            var adAdmin = resources.OfType<ActiveDirectoryAdministrator>().FirstOrDefault();
            Assert.NotNull(adAdmin);
            
            var principalId = await adAdmin.ObjectId.GetValueAsync();
            Assert.NotNull(principalId);
            Assert.Equal(appServiceIdentity.PrincipalId, principalId);
        }
        
        [Fact]
        public async Task ShouldSetLogin()
        {
            var resources = await _stackHelper.TestAsync();
            
            var adAdmin = resources.OfType<ActiveDirectoryAdministrator>().FirstOrDefault();
            Assert.NotNull(adAdmin);
            
            var login = await adAdmin.Login.GetValueAsync();
            Assert.NotNull(login);
            Assert.Equal("libraryadmin", login);
        }
        
        [Fact]
        public async Task ShouldSetSqlServerName()
        {
            var resources = await _stackHelper.TestAsync();
            
            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var sqlServerName = await sqlServer.Name.GetValueAsync();
            
            var adAdmin = resources.OfType<ActiveDirectoryAdministrator>().FirstOrDefault();
            Assert.NotNull(adAdmin);
            
            var serverName = await adAdmin.ServerName.GetValueAsync();
            Assert.NotNull(serverName);
            Assert.Equal(sqlServerName, serverName);
        }
    }
}