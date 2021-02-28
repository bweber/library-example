using System.Linq;
using System.Threading.Tasks;
using Pulumi.Azure.KeyVault;
using Pulumi.Azure.Sql;
using Pulumi.Random;
using Xunit;

namespace Library.Infrastructure.Tests.Tests
{
    public class ConnectionStringSecretTests
    {
        private readonly MockStackHelper _stackHelper;

        public ConnectionStringSecretTests()
        {
            _stackHelper = new MockStackHelper();
        }
        
        [Fact]
        public async Task ShouldSetName()
        {
            var resources = await _stackHelper.TestAsync();
            
            var secret = resources.OfType<Secret>().FirstOrDefault();
            Assert.NotNull(secret);

            var name = await secret.Name.GetValueAsync();
            Assert.NotNull(name);
            Assert.Equal("ConnectionStrings--LibraryDB", name);
        }
        
        [Fact]
        public async Task ShouldSetKeyVaultId()
        {
            var resources = await _stackHelper.TestAsync();
            
            var keyVault = resources.OfType<KeyVault>().FirstOrDefault();
            Assert.NotNull(keyVault);
            
            var vaultId = await keyVault.Id.GetValueAsync();
            
            var secret = resources.OfType<Secret>().FirstOrDefault();
            Assert.NotNull(secret);

            var keyVaultId = await secret.KeyVaultId.GetValueAsync();
            Assert.NotNull(keyVaultId);
            Assert.Equal(vaultId, keyVaultId);
        }
        
        [Fact]
        public async Task ShouldSetConnectionStringAsValue()
        {
            var resources = await _stackHelper.TestAsync();
            
            var sqlServer = resources.OfType<SqlServer>().FirstOrDefault();
            Assert.NotNull(sqlServer);

            var sqlServerName = await sqlServer.Name.GetValueAsync();
            
            var database = resources.OfType<Database>().FirstOrDefault();
            Assert.NotNull(database);

            var databaseName = await database.Name.GetValueAsync();
            
            var randomLogin = resources.OfType<RandomString>().FirstOrDefault();
            Assert.NotNull(randomLogin);
            
            var randomAdminLogin = await randomLogin.Result.GetValueAsync();
            
            var randomPassword = resources.OfType<RandomPassword>().FirstOrDefault();
            Assert.NotNull(randomPassword);
            
            var randomAdminPassword = await randomPassword.Result.GetValueAsync();
            
            var secret = resources.OfType<Secret>().FirstOrDefault();
            Assert.NotNull(secret);

            var value = await secret.Value.GetValueAsync();
            Assert.NotNull(value);
            Assert.Equal($"Server=tcp:{sqlServerName}.database.windows.net;Database={databaseName};User Id={randomAdminLogin};Password={randomAdminPassword};", value);
        }
        
        [Fact]
        public async Task ShouldSetEnvironmentTag()
        {
            var resources = await _stackHelper.TestAsync();

            var secret = resources.OfType<Secret>().FirstOrDefault();
            Assert.NotNull(secret);
            
            var tags = await secret.Tags.GetValueAsync();
            Assert.NotNull(tags);
            Assert.True(tags.ContainsKey("environment"));
            Assert.Equal("dev", tags["environment"]);
        }
    }
}