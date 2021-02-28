using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.Sql;
using Pulumi.Random;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class SqlServerTests
    {
        private readonly MockStackHelper _stackHelper;
        
        public SqlServerTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetResourceGroupName()
        {
            var resources = await _stackHelper.TestAsync();

            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var resourceGroupName = await sqlServer.ResourceGroupName.GetValueAsync();
            Assert.NotNull(resourceGroupName);
            Assert.Equal("library-rg", resourceGroupName);
        }
        
        [Fact]
        public async Task ShouldSetLocationFromResourceGroup()
        {
            var resources = await _stackHelper.TestAsync();

            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var location = await sqlServer.Location.GetValueAsync();
            Assert.NotNull(location);
            Assert.Equal("CentralUS", location);
        }
        
        [Fact]
        public async Task ShouldSetRandomlyGeneratedLogin()
        {
            var resources = await _stackHelper.TestAsync();

            var randomLogin = resources.OfType<RandomString>().FirstOrDefault();
            Assert.NotNull(randomLogin);

            var loginLength = await randomLogin.Length.GetValueAsync();
            Assert.Equal(16, loginLength);
            
            var loginSpecial = await randomLogin.Special.GetValueAsync();
            Assert.False(loginSpecial);
            
            var loginNumber = await randomLogin.Number.GetValueAsync();
            Assert.False(loginNumber);
            
            var randomAdminLogin = await randomLogin.Result.GetValueAsync();
            
            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var adminLogin = await sqlServer.AdministratorLogin.GetValueAsync();
            Assert.NotNull(adminLogin);
            Assert.Equal(randomAdminLogin, adminLogin);
        }
        
        [Fact]
        public async Task ShouldSetRandomlyGeneratedPassword()
        {
            var resources = await _stackHelper.TestAsync();

            var randomPassword = resources.OfType<RandomPassword>().FirstOrDefault();
            Assert.NotNull(randomPassword);

            var passwordLength = await randomPassword.Length.GetValueAsync();
            Assert.Equal(32, passwordLength);
            
            var passwordSpecial = await randomPassword.Special.GetValueAsync();
            Assert.True(passwordSpecial);
            
            var randomAdminPassword = await randomPassword.Result.GetValueAsync();
            
            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var adminPassword = await sqlServer.AdministratorLoginPassword.GetValueAsync();
            Assert.NotNull(adminPassword);
            Assert.Equal(randomAdminPassword, adminPassword);
        }
        
        [Fact]
        public async Task ShouldSetLatestVersion()
        {
            var resources = await _stackHelper.TestAsync();

            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var version = await sqlServer.Version.GetValueAsync();
            Assert.NotNull(version);
            Assert.Equal("12.0", version);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);
            
            var tags = await sqlServer.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}